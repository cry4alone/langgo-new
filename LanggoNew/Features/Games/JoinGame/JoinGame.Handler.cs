using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using StackExchange.Redis;

namespace LanggoNew.Features.Games.JoinGame;

public class Handler(IRedisCache cache, ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = int.Parse(currentUserService.GetCurrentUserId());
        
        await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {

            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);

            if (!currentGameState.PlayerUserIds.Contains(currentUserId))
                currentGameState.PlayerUserIds.Add(currentUserId);
               
            await cache.SetDataAsync(gameKey, currentGameState);
        });
    }
}