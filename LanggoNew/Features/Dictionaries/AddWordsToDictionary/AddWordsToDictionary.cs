using System.Security.Claims;
using FluentValidation;
using LanggoNew.Endpoints;
using LanggoNew.Models;
using LanggoNew.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Dictionaries;

public class AddWordsToDictionary
{
            

    
    public record Request(
        List<DictionaryEntry> WordsWithTranslations);
    
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.WordsWithTranslations)
                .NotEmpty()
                .Must(r => r.Count <= 100)
                .WithMessage("You can add up to 100 words at a time.");
            RuleForEach(r => r.WordsWithTranslations).SetValidator(new DictionaryEntryValidator());
        }
    }
    
    public class DictionaryEntryValidator : AbstractValidator<DictionaryEntry>
    {
        public DictionaryEntryValidator()
        {
            RuleFor(e => e.Original).NotEmpty();
            RuleFor(e => e.Translation).NotEmpty();
            RuleFor(e => e.Difficulty).InclusiveBetween(1, 5);
        }
    }
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/dictionary/{id}/add-words", Handler)
                .WithTags("Dictionary")
                .RequireAuthorization();
        }
    }
    
    public static async Task<Results<Created, BadRequest<string>>> Handler(
        int id,
        Request request,
        IValidator<Request> validator,
        HttpContext httpContext,
        AppDbContext context)
    {
        await validator.ValidateAndThrowAsync(request);
        
        var dictionary = await context.Dictionaries.FindAsync(id);
        if (dictionary == null) return TypedResults.BadRequest("Dictionary not found.");
        
        var currentUserId = httpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        if (dictionary.OwnerId != int.Parse(currentUserId)) 
            return TypedResults.BadRequest("You do not have permission to modify this dictionary.");

        var wordsToAdd = request.WordsWithTranslations.Select(entry => new DictionaryWord
        {
            Original = entry.Original,
            Translation = entry.Translation,
            Example = entry.Example,
            Difficulty = entry.Difficulty,
            DictionaryId = id
        }).ToList();

        await context.DictionaryWords.AddRangeAsync(wordsToAdd);
        await context.SaveChangesAsync();
            
        return TypedResults.Created();
    }
}