using System.Text.Json;
using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public class Endpoint : IEndpoint
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dictionary/upload", async (IFormFile file, ISender sender) =>
        {
            if (file.Length == 0)
                return Results.BadRequest("No file uploaded");
            
            if (file.ContentType != "application/json")
                return Results.BadRequest("Invalid file type. Please upload a JSON file.");
            
            await using var stream = file.OpenReadStream();
            var request = await JsonSerializer.DeserializeAsync<Request>(stream, _jsonOptions);
            
            if (request == null)
                return Results.BadRequest("Invalid file content");

            await sender.Send(request);
            
            return Results.Ok("File uploaded successfully");
        })
        .DisableAntiforgery()
        .WithTags("Dictionary")
        .RequireAuthorization();
    }
}

