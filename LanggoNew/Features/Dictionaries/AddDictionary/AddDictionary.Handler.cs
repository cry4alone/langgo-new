using AutoMapper;
using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public class Handler(
    AppDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService) : IRequestHandler<Request>
{
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        
        var newDictionary = new Dictionary();
        mapper.Map(request, newDictionary);
        
        newDictionary.CreatedAt = DateTime.UtcNow;
        newDictionary.OwnerId = currentUserId;
        
        context.Dictionaries.Add(newDictionary);
        
        var entries = request.WordsWithTranslations ?? new List<DictionaryEntry>();
        var wordsToAdd = entries.Select(entry => new DictionaryWord
        {
            Original = entry.Original,
            Translation = entry.Translation,
            Example = entry.Example,
            Difficulty = entry.Difficulty,
            Dictionary = newDictionary
        }).ToList();

        if (wordsToAdd.Count > 0)
        {
            await context.DictionaryWords.AddRangeAsync(wordsToAdd, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
