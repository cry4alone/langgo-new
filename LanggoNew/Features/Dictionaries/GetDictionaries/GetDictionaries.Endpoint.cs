using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Dictionaries.GetDictionaries;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries", async (ISender sender) =>
        {
            var response = await sender.Send(new Request());
            return TypedResults.Ok(response);
        })
        .WithTags("Dictionary");
    }
}

