using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.User.GetUserDictionaries;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/user/{userId}/dictionaries", async (int userId, ISender sender) =>
        {
            var response = await sender.Send(new Request(userId));
            return TypedResults.Ok(response);
        }).RequireAuthorization().WithTags("User");
    }
}