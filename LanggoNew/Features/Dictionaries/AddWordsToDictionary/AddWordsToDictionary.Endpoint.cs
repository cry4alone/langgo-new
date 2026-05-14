using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddWordsToDictionary;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dictionary/{id}/add-words", async (int id, Request request, ISender sender) =>
        {
            var requestWithId = request with { DictionaryId = id };
            await sender.Send(requestWithId);
            return Results.Created();
        })
        .WithTags("Dictionary")
        .RequireAuthorization();
    }
}

