using LanggoNew.Shared.Config;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LanggoNew.Features.Games.StartNewRound;
public record Command(string RoomId) : IRequest;
public record Response(string NewWord, int RoundNumber, DateTime TimeForRoundSeconds);

public class Handler(
    IRedisCache cache,
    ISender sender,
    IGameTimerService timerService,
    IOptions<GameTimingOptions> timingOptions,
    IHubContext<GameHub> hubContext) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var roomId = request.RoomId;

        await cache.ExecuteWithLockAsync(roomId, async gameKey =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey)
                ?? throw new NotFoundException($"Game state not found for room '{roomId}'");

            var hasNextByRounds = currentGameState.CurrentRound + 1 <= currentGameState.MaxRounds;
            if (!hasNextByRounds)
            {
                await sender.Send(new EndGame.Command(roomId, currentGameState), cancellationToken);
                return;
            }
        
            currentGameState.CurrentWordData = currentGameState.GameWords[currentGameState.CurrentRound];
            var currentRound = ++currentGameState.CurrentRound;
            var newWord = currentGameState.CurrentWordData.Original;

            currentGameState.IsRoundEnding = false;
            
            currentGameState.CurrentJobId = await timerService.ScheduleEndRound(roomId);
            var endRoundTime = DateTime.UtcNow.AddSeconds(timingOptions.Value.RoundDurationSeconds);
            currentGameState.CurrentJobEndTimeUtc = endRoundTime;
            
            await cache.SetDataAsync(gameKey, currentGameState);
            
            await hubContext.Clients.Group(roomId).SendAsync("StartNewRound",
                new Response(newWord,
                    currentRound,
                    endRoundTime),
                cancellationToken);
        });
        
    }
}
