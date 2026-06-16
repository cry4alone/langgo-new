using LanggoNew.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface IWordService
{
    Task<List<WordData>> GetRandomWordsFromDictionary(int count, int dictionaryId);
    Task<List<string>> GetWrongTranslations(int excludeWordId, int dictionaryId, int count);
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
                DictionaryWordId = w.Id,
                Original = w.Original,
                Translation = w.Translation,
                Example = w.Example
            })
            .ToListAsync();
    }

    public async Task<List<string>> GetWrongTranslations(int excludeWordId, int dictionaryId, int count)
    {
        return await context.DictionaryWords
            .Where(w => w.DictionaryId == dictionaryId && w.Id != excludeWordId)
            .OrderBy(w => EF.Functions.Random())
            .Take(count)
            .Select(w => w.Translation)
            .ToListAsync();
    }
}