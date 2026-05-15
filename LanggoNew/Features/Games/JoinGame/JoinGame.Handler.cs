using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using StackExchange.Redis;

namespace LanggoNew.Features.Games.JoinGame;

public class Handler(IRedisCache cache, ICurrentUserService currentUserService, AppDbContext context) : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        var currentUser = context.Users.FirstOrDefault(x => x.Id == currentUserId);
        if (currentUser == null)
            throw new NotFoundException("User not found.");
        
        var isHost = false;
        
        await cache.ExecuteWithLockAsync(request.RoomId, async (gameKey) =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            if (currentGameState == null)
                throw new NotFoundException("GameState not found.");
            
            isHost = int.Parse(currentGameState.HostUserId) == currentUserId;
            
            if(currentGameState.Status == GameStatus.InProgress) 
                throw new InvalidOperationException("Cannot join a game that is already in progress.");   
            
            if (!currentGameState.PlayerUserIds.Contains(currentUserId))
                currentGameState.PlayerUserIds.Add(currentUserId);
               
            await cache.SetDataAsync(gameKey, currentGameState);
        });

        return new Response(
            currentUserId,
            currentUser.Username,
            isHost,
            currentUser.Avatar,
            currentUser.NativeLanguage,
            currentUser.Rating
        );
    }
}