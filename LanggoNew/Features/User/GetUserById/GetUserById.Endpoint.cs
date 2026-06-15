using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.User.GetUserById;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/{userId:int}", async (int userId, ISender sender) =>
        {
            var query = new Query(userId);
            var response = await sender.Send(query);

            if (response is null)
                return Results.NotFound();

            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("User");
    }
}
