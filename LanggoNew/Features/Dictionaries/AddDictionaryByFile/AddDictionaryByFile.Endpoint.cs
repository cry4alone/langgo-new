using System.Text.Json;
using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dictionary/upload", async Task<Results<BadRequest<string>, Ok<string>>> (IFormFile file, ISender sender) =>
        {
            if (file.Length == 0)
            {
                return TypedResults.BadRequest("No file uploaded");
            }
            
            if(file.ContentType != "application/json")
            {
                return TypedResults.BadRequest("Invalid file type. Please upload a JSON file.");
            }
            
            await using var stream = file.OpenReadStream();
            var command = await JsonSerializer.DeserializeAsync<Command>(stream);

            await sender.Send(command);
            
            return TypedResults.Ok("File uploaded successfully");
        }).DisableAntiforgery().WithTags("Dictionary").RequireAuthorization();
    }
}