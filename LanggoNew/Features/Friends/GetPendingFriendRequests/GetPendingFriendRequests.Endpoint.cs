using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Friends.GetPendingFriendRequests;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("friends/requests/incoming", async (ISender sender) =>
        {
            var response = await sender.Send(new Query());
            return TypedResults.Ok(response);
        })
        .WithTags("Friends")
        .RequireAuthorization();
    }
}
