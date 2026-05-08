using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Authentication.Logout;

public class Endpoint(ISender sender) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (Command request) =>
        {
            await sender.Send(request);
        })
        .RequireAuthorization()
        .WithTags("Authentication");
    }
}