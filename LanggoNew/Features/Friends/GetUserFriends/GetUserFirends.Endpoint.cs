using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.Friends.GetUserFriends;

public class Enpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("friends/{userId:int}", async (int userId, ISender sender) =>
        {
            var response = await sender.Send(new Query(userId));

            return TypedResults.Ok(response);
        }).WithTags("Friends");
    }
}