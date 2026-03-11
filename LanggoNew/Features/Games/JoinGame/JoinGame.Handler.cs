using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Models;
using MediatR;
using StackExchange.Redis;

namespace LanggoNew.Features.Games.JoinGame;

public class Handler(IRedisCache cache) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await cache.ExecuteWithLockAsync(request.RoomId, request.UserId, async () =>
        {
            var gameKey = $"game:{request.RoomId}";
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            currentGameState.PlayerUserIds.Add(request.UserId);
            await cache.SetDataAsync(gameKey, currentGameState);
        });
    }
}