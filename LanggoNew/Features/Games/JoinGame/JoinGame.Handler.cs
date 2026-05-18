using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LanggoNew.Shared.DTO;

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
        
        var gameData = await cache.ExecuteWithLockAsync(request.RoomId,
            async (gameKey) =>
            {
                var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
                if (currentGameState == null)
                    throw new NotFoundException("GameState not found.");

                if (!int.TryParse(currentGameState.HostUserId,
                        out var hostUserId))
                    throw new InvalidOperationException("Invalid HostUserId format in game state.");

                if (currentGameState.Status == GameStatus.InProgress)
                    throw new InvalidOperationException("Cannot join a game that is already in progress.");

                if (currentGameState.PlayerUserIds.Count >= MaxPlayersPerGame &&
                    !currentGameState.PlayerUserIds.Contains(currentUserId))
                    throw new InvalidOperationException("Game is full. Cannot join.");

                if (!currentGameState.PlayerUserIds.Contains(currentUserId))
                    currentGameState.PlayerUserIds.Add(currentUserId);

                await cache.SetDataAsync(gameKey,
                    currentGameState);

                return new
                {
                    HostUserId = hostUserId,
                    DictionaryId = currentGameState.DictionaryId,
                    MaxRounds = currentGameState.MaxRounds,
                    PlayerIds = currentGameState.PlayerUserIds.ToList()
                };
            });
        
        var currentGamePlayers = await context.Users
            .Where(u => gameData.PlayerIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
        
        var mappedPlayers = mapper.Map<List<UserData>>(currentGamePlayers)
            .Select(p => p with { IsHost = p.UserId == gameData.HostUserId })
            .ToList();
        
        var newPlayerInfo = mapper.Map<UserData>(currentUser) with { IsHost = currentUser.Id == gameData.HostUserId };

        await hubContext.Clients.GroupExcept(request.RoomId, [request.ConnectionId])
            .SendAsync("PlayerJoined", newPlayerInfo, cancellationToken);

        var dictionary = await context.Dictionaries
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == gameData.DictionaryId, cancellationToken);

        if (dictionary == null)
            throw new NotFoundException("Dictionary not found.");

        if (!Enum.TryParse(dictionary.LangFrom, true, out LanguageCode langFrom) ||
            !Enum.TryParse(dictionary.LangTo, true, out LanguageCode langTo))
            throw new InvalidOperationException("Invalid language codes.");
        
        var gameSettings = new GameSettings(dictionary.Name, langFrom, langTo, gameData.MaxRounds);
        
        return new Response(mappedPlayers, gameSettings);
    }
}