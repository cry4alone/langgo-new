using LanggoNew.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface IWordService
{
    Task<List<WordData>> GetRandomWordsFromDictionary(int count, int dictionaryId);
}

public class WordService(AppDbContext context) : IWordService
{
    public async Task<List<WordData>> GetRandomWordsFromDictionary(int count, int dictionaryId)
    {
        return await context.DictionaryWords
            .Where(w => w.DictionaryId == dictionaryId)
            .OrderBy(w => EF.Functions.Random())
            .Take(count)
            .Select(w => new WordData
            {
                Original = w.Original,
                Translation = w.Translation,
                Example = w.Example
            })
            .ToListAsync();
    }
}