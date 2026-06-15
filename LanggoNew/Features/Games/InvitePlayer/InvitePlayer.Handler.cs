using LanggoNew.Shared.DTO;
using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Games.InvitePlayer;

public class Handler(
    IRedisCache cache,
    ICurrentUserService currentUserService,
    AppDbContext context,
    IHubContext<Notifications.NotificationHub> notificationHub) : IRequestHandler<Command>
{
    private const int MaxPlayersPerGame = 4;

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();

        var gameState = await cache.GetDataAsync<GameState>($"game:{request.RoomId}");
        if (gameState is null)
            throw new NotFoundException("Game");

        if (gameState.Status != GameStatus.Waiting)
            throw new ConflictException("Game is not in Waiting status.");

        if (!int.TryParse(gameState.HostUserId, out var hostUserId) || hostUserId != currentUserId)
            throw new ConflictException("Only the host can invite players.");

        if (request.TargetUserId == currentUserId)
            throw new ConflictException("Cannot invite yourself.");

        if (gameState.PlayerUserIds.Contains(request.TargetUserId))
            throw new ConflictException("Player is already in the game.");

        if (gameState.PlayerUserIds.Count >= MaxPlayersPerGame - 1)
            throw new ConflictException("Game is full.");

        if (gameState.PendingInvites.Contains(request.TargetUserId))
            throw new ConflictException("Invite already sent to this player.");

        gameState.PendingInvites.Add(request.TargetUserId);
        await cache.SetDataAsync($"game:{request.RoomId}", gameState);

        var hostUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

        var dictionary = await context.Dictionaries
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == gameState.DictionaryId, cancellationToken);

        if (dictionary is null)
            throw new NotFoundException("Dictionary");

        if (!Enum.TryParse(dictionary.LangFrom, true, out LanguageCode langFrom) ||
            !Enum.TryParse(dictionary.LangTo, true, out LanguageCode langTo))
            throw new InvalidOperationException("Invalid language codes.");

        var gameSettings = new GameSettings(dictionary.Name, langFrom, langTo, gameState.MaxRounds);

        await notificationHub.Clients
            .User(request.TargetUserId.ToString())
            .SendAsync("GameInviteReceived", new
            {
                roomId = request.RoomId,
                hostUserId = currentUserId,
                hostUsername = hostUser?.Username ?? string.Empty,
                gameSettings
            }, cancellationToken);
    }
}
