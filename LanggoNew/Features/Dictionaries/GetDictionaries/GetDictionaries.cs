using LanggoNew.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Dictionaries.GetDictionaries;

public static class GetDictionaries
{
    public record Response(
        int Id, 
        string Name, 
        string LangFrom, 
        string LangTo, 
        string Description,
        int WordsCount);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/dictionaries", Handler).WithTags("Dictionary");
        }
    }
    
    public static async Task<IResult> Handler(AppDbContext context)
    {
        var wordsCounts = await context.DictionaryWords
            .GroupBy(dw => dw.DictionaryId)
            .Select(g => new { DictionaryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DictionaryId, x => x.Count);

        var responses = await context.Dictionaries
            .Where(d => d.IsPublic == true)
            .Select(d => new Response(
                d.Id, 
                d.Name, 
                d.LangFrom, 
                d.LangTo, 
                d.Description,
                wordsCounts.GetValueOrDefault(d.Id, 0)))
            .ToListAsync();
        
        return TypedResults.Ok(responses);
    }
}