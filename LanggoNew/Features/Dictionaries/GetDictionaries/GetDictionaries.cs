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
        string Description);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/dictionaries", Handler).WithTags("Dictionary");
        }
    }
    
    public static async Task<IResult> Handler(AppDbContext context)
    {
        var dictionaries = await context.Dictionaries
            .Where(d => d.IsPublic == true)
            .ToListAsync();

        var responses = dictionaries.Select(d => new Response(
            d.Id, 
            d.Name, 
            d.LangFrom, 
            d.LangTo, 
            d.Description)).ToList();

        return TypedResults.Ok(responses);
    }
}