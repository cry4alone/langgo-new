using System.Security.Claims;
using LanggoNew.Models;
using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace LanggoNew.Features.Games.CreateGame;

public class Handler(AppDbContext context, ICurrentUserService currentUserService, IRedisCache cache) : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingDictionary = await context.Dictionaries.FirstOrDefaultAsync(d => d.Id == request.DictionaryId, cancellationToken: cancellationToken);
        if (existingDictionary is null)
            throw new DictionaryNotFoundException(request.DictionaryId);
        
        var newGame = new Game()
        {
            DictionaryId = request.DictionaryId,
            Mode = Enum.GetName(typeof(GameMode), request.Mode),
            RoundsCount = request.MaxRounds,
            Status = nameof(GameStatus.Waiting),
            CreatedAt = DateTime.UtcNow
        };
        context.Games.Add(newGame);
        await context.SaveChangesAsync(cancellationToken);

        var currentUserId = currentUserService.GetCurrentUserId();
        
        var gameState = new GameState
        {
            RoomId = Guid.NewGuid().ToString(),
            GameId = newGame.Id,
            HostUserId = currentUserId,
            PlayerUserIds = [int.Parse(currentUserId)],
            MaxRounds = request.MaxRounds,
        };
        await cache.SetDataAsync(
            $"game:{gameState.RoomId}",
            gameState);

        return new Response(gameState.RoomId);
    }
}