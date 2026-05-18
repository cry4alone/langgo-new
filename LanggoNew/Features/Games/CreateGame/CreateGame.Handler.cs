using LanggoNew.Models;
using LanggoNew.Shared.DTO;
using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Enum;

namespace LanggoNew.Features.Games.CreateGame;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService,
    IRedisCache cache,
    IWordService wordService) : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingDictionary = await context.Dictionaries.FirstOrDefaultAsync(d => d.Id == request.DictionaryId, cancellationToken: cancellationToken);
        if (existingDictionary is null)
            throw new DictionaryNotFoundException(request.DictionaryId);
        
        var newGame = new Game()
        {
            DictionaryId = request.DictionaryId,
            Mode = GetName(typeof(GameMode), request.Mode),
            RoundsCount = request.MaxRounds,
            Status = nameof(GameStatus.Waiting),
            CreatedAt = DateTime.UtcNow
        };
        context.Games.Add(newGame);
        await context.SaveChangesAsync(cancellationToken);

        var currentUserId = currentUserService.GetCurrentUserId().ToString();
        
        var words = await wordService.GetRandomWordsFromDictionary(
            request.MaxRounds,
            request.DictionaryId);
        
        if(words.Count < request.MaxRounds)
            throw new NotEnoughWordsInDictionaryException(request.DictionaryId, request.MaxRounds);
        
        var gameState = new GameState
        {
            RoomId = Guid.NewGuid().ToString(),
            GameId = newGame.Id,
            DictionaryId = request.DictionaryId,
            GameWords = words,
            HostUserId = currentUserId,
            PlayerUserIds = [],
            MaxRounds = request.MaxRounds,
        };
        
        if (!TryParse(existingDictionary.LangFrom, true, out LanguageCode langFrom) ||
            !TryParse(existingDictionary.LangTo, true, out LanguageCode langTo))
            throw new InvalidOperationException("Invalid language codes.");

        
        Console.WriteLine($"Langs before{existingDictionary.LangFrom} {existingDictionary.LangTo} langs after {langFrom} {langTo}");
        
        await cache.SetDataAsync(
            $"game:{gameState.RoomId}",
            gameState);
        
        return new Response(gameState.RoomId, new GameSettings(existingDictionary.Name,
            langFrom,
            langTo,
            request.MaxRounds));
    }

}