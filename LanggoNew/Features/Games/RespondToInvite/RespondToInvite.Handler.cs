using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LanggoNew.Features.Games.RespondToInvite;

public class Handler(
    IRedisCache cache,
    ICurrentUserService currentUserService,
    IHubContext<Notifications.NotificationHub> notificationHub) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();

        var gameState = await cache.GetDataAsync<GameState>($"game:{request.RoomId}");
        if (gameState is null)
            throw new NotFoundException("Game");

        if (gameState.Status != GameStatus.Waiting)
            throw new ConflictException("Game is not in Waiting status.");

        if (!gameState.PendingInvites.Contains(currentUserId))
            throw new ConflictException("No pending invite for this game.");

        gameState.PendingInvites.Remove(currentUserId);
        await cache.SetDataAsync($"game:{request.RoomId}", gameState);

        if (!request.Accept)
        {
            await notificationHub.Clients
                .User(gameState.HostUserId)
                .SendAsync("GameInviteDeclined", new
                {
                    roomId = request.RoomId,
                    invitedUserId = currentUserId
                }, cancellationToken);
        }
    }
}
