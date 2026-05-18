using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Dictionaries.DeleteDictionary;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/dictionary/{id}", async (int id, ISender sender) =>
        {
            await sender.Send(new Request(id));
            return Results.NoContent();
        })
        .WithTags("Dictionary")
        .RequireAuthorization();
    }
}

