using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.User.ChangePassword;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user/change-password", async (Command request, ISender sender) =>
        {
            await sender.Send(request);
            return TypedResults.NoContent();
        })
        .RequireAuthorization()
        .WithTags("User");
    }
}