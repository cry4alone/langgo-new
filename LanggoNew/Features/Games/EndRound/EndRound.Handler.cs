using LanggoNew.Shared.Config;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LanggoNew.Features.Games.EndRound;

public record Command(string RoomId) : IRequest;
public record Response(int? WinnerId, Dictionary<int, int> Scores, DateTime NewRoundTime);

public class Handler(
    IRedisCache cache,
    IGameTimerService timerService,
    IOptions<GameTimingOptions> timingOptions,
    IHubContext<GameHub> hubContext) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await cache.ExecuteWithLockAsync(request.RoomId, async gameKey =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey)
                ?? throw new NotFoundException($"Game state not found for room '{request.RoomId}'");

            var scores = new Dictionary<int, int>(currentGameState.UserScores);
            int? winnerId = currentGameState.RoundWinners.TryPeek(out var roundWinnerId)
                ? roundWinnerId
                : null;

            if (currentGameState.LastEndedRound == currentGameState.CurrentRound)
            {
                return;
            }

            await timerService.CancelJob(currentGameState.CurrentJobId);

            currentGameState.IsRoundEnding = true;
            currentGameState.LastEndedRound = currentGameState.CurrentRound;

            currentGameState.CurrentJobId = await timerService.ScheduleStartNewRound(request.RoomId);
            var newRoundTime = DateTime.UtcNow.AddSeconds(timingOptions.Value.PauseBetweenRoundsSeconds);
            currentGameState.CurrentJobEndTimeUtc = newRoundTime;

            await cache.SetDataAsync(gameKey, currentGameState);
            
            await hubContext.Clients.Group(request.RoomId).SendAsync("EndRound",
                new Response(winnerId,
                    scores,
                    newRoundTime),
                cancellationToken);
        });
    }
}