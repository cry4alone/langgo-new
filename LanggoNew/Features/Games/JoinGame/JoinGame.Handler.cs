using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Games.JoinGame;

public class Handler(IRedisCache cache, ICurrentUserService currentUserService, AppDbContext context) : IRequestHandler<Command, Response>
{
    private const int MaxPlayersPerGame = 4; // Можно переместить в конфиг
    
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        
        // Получаем пользователя из БД асинхронно
        var currentUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentUserId, cancellationToken);
        
        if (currentUser == null)
            throw new NotFoundException("User not found.");
        
        var isHost = false;
        var currentGamePlayers = new List<int>();
        
        await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            if (currentGameState == null)
                throw new NotFoundException("GameState not found.");
            
            if (!int.TryParse(currentGameState.HostUserId, out var hostUserId))
                throw new InvalidOperationException("Invalid HostUserId format in game state.");
            
            isHost = hostUserId == currentUserId;
            
            if (currentGameState.Status == GameStatus.InProgress) 
                throw new InvalidOperationException("Cannot join a game that is already in progress.");
            
            if (currentGameState.PlayerUserIds.Count >= MaxPlayersPerGame && 
                !currentGameState.PlayerUserIds.Contains(currentUserId))
                throw new InvalidOperationException("Game is full. Cannot join.");
            
            if (!currentGameState.PlayerUserIds.Contains(currentUserId))
                currentGameState.PlayerUserIds.Add(currentUserId);
            
            currentGamePlayers = new List<int>(currentGameState.PlayerUserIds);
            
            await cache.SetDataAsync(gameKey, currentGameState);
        });

        return new Response(
            currentUserId,
            currentUser.Username,
            isHost,
            currentUser.Avatar,
            currentUser.NativeLanguage,
            currentUser.Rating,
            currentGamePlayers
        );
    }
}