using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using LanggoNew.Endpoints;
using LanggoNew.Models;
using LanggoNew.Shared.Enum;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public static class AddDictionary
{
    public record Request(
        string Name, 
        LanguageCode LangFrom, 
        LanguageCode LangTo,
        string Description,
        bool IsPublic);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.LangFrom).NotEmpty();
            RuleFor(r => r.LangTo).NotEmpty();
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/dictionary", Handler)
                .WithTags("Dictionary")
                .RequireAuthorization();
        }
    }

    public static async Task<Results<Created, BadRequest>> Handler(
        Request request, 
        IValidator<Request> validator,
        IMapper mapper,
        AppDbContext context,
        HttpContext httpContext)
    {
        await validator.ValidateAndThrowAsync(request);
        
        var newDictionary = new Dictionary();
        mapper.Map(request, newDictionary);
        
        var currentUserId = httpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        
        newDictionary.CreatedAt = DateTime.UtcNow;
        newDictionary.OwnerId = int.Parse(currentUserId);
        
        context.Dictionaries.Add(newDictionary);
        await context.SaveChangesAsync();
        
        return TypedResults.Created();
    }
}