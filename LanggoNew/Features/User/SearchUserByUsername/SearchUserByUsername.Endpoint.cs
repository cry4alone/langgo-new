using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.User.SearchUserByUsername;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/search", async (string username, ISender sender) =>
        {
            var response = await sender.Send(new Query(username));
            return TypedResults.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("User");
    }
}
