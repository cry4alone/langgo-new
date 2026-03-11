using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Games.LeaveGame;

public class Handler(IRedisCache cache) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await cache.ExecuteWithLockAsync(request.RoomId, request.UserId, async () =>
        {
            var gameKey = $"game:{request.RoomId}";
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            currentGameState.PlayerUserIds.Remove(request.UserId);
            // если игрков не осталось, заканчиваем игру
            await cache.SetDataAsync(gameKey, currentGameState);
        });
    }
}