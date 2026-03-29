using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Games.LeaveGame;

public class Handler(IRedisCache cache, ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = int.Parse(currentUserService.GetCurrentUserId());
        
        await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            currentGameState.PlayerUserIds.Remove(currentUserId);
            // если игрков не осталось, заканчиваем игру
            await cache.SetDataAsync(gameKey, currentGameState);
        });
    }
}