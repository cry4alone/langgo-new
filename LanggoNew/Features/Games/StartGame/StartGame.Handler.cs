using LanggoNew.Shared.Config;
using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using LanggoNew.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LanggoNew.Features.Games.StartGame;

public class Handler(
    IRedisCache cache,
    IGameTimerService timerService,
    IOptions<GameTimingOptions> timingOptions) : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        return await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {
            var gameState = await cache.GetDataAsync<GameState>(gameKey);
            if (gameState is null)
                throw new NotFoundException("GameState not found.");
            
            gameState.Status = GameStatus.InProgress;
            
            gameState.CurrentJobId = await timerService.ScheduleStartNewRound(request.RoomId);
            var newRoundTime = DateTime.UtcNow.AddSeconds(timingOptions.Value.PauseBetweenRoundsSeconds);
            gameState.CurrentJobEndTimeUtc = newRoundTime;
            
            await cache.SetDataAsync(gameKey, gameState);
            return new Response(newRoundTime);
        });
    }
}