using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/dictionary", async (Request request, ISender sender) =>
        {
            await sender.Send(request);
            return TypedResults.Created();
        })
        .WithTags("Dictionary")
        .RequireAuthorization();
    }
}

