using LanggoNew.Models;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddWordsToDictionary;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Request>
{
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
        var dictionary = await context.Dictionaries.FindAsync(
            new object[] { request.DictionaryId }, 
            cancellationToken: cancellationToken);
        
        if (dictionary == null)
            throw new NotFoundException("Dictionary not found.");
        
        var currentUserId = currentUserService.GetCurrentUserId();
        if (dictionary.OwnerId != currentUserId)
            throw new InvalidOperationException("You do not have permission to modify this dictionary.");

        var wordsToAdd = request.WordsWithTranslations.Select(entry => new DictionaryWord
        {
            Original = entry.Original,
            Translation = entry.Translation,
            Example = entry.Example,
            Difficulty = entry.Difficulty,
            DictionaryId = request.DictionaryId
        }).ToList();

        await context.DictionaryWords.AddRangeAsync(wordsToAdd, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}


