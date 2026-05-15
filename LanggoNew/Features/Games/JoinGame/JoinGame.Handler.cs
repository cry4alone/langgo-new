using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LanggoNew.Features.Games.JoinGame;

public class Handler(
    IRedisCache cache,
    ICurrentUserService currentUserService,
    AppDbContext context,
    IHubContext<GameHub> hubContext,
    IMapper mapper) : IRequestHandler<Command, Response>
{
    private const int MaxPlayersPerGame = 4; 
    
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        
        var currentUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentUserId, cancellationToken);
        
        if (currentUser == null)
            throw new NotFoundException("User not found.");
        
        var playerIds = await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            if (currentGameState == null)
                throw new NotFoundException("GameState not found.");
            
            if (!int.TryParse(currentGameState.HostUserId, out var hostUserId))
                throw new InvalidOperationException("Invalid HostUserId format in game state.");
            
            if (currentGameState.Status == GameStatus.InProgress) 
                throw new InvalidOperationException("Cannot join a game that is already in progress.");
            
            if (currentGameState.PlayerUserIds.Count >= MaxPlayersPerGame && 
                !currentGameState.PlayerUserIds.Contains(currentUserId))
                throw new InvalidOperationException("Game is full. Cannot join.");
            
            if (!currentGameState.PlayerUserIds.Contains(currentUserId))
                currentGameState.PlayerUserIds.Add(currentUserId);
            
            await cache.SetDataAsync(gameKey, currentGameState);
            
            return new { HostUserId = hostUserId, PlayerIds = currentGameState.PlayerUserIds.ToList() };
        });
        
        var currentGamePlayers = await context.Users
            .Where(u => playerIds.PlayerIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
        
        var mappedPlayers = mapper.Map<List<UserData>>(currentGamePlayers)
            .Select(p => p with { IsHost = p.UserId == playerIds.HostUserId })
            .ToList();
        
        var newPlayerInfo = mapper.Map<UserData>(currentUser) with { IsHost = currentUser.Id == playerIds.HostUserId };

        await hubContext.Clients.GroupExcept(request.RoomId, [request.ConnectionId])
            .SendAsync("PlayerJoined", newPlayerInfo, cancellationToken);
        
        return new Response(mappedPlayers);
    }
}