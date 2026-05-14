using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.User.GetAvatar;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/avatar-url", async (string key, ISender sender) =>
        {
            var request = new Request(key);
            var response = await sender.Send(request);
            return TypedResults.Ok(response);
        })
        .WithTags("User")
        .RequireAuthorization();
    }
}

