using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Authentication.Logout;

public class Endpoint() : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (Command request, ISender sender) =>
        {
            await sender.Send(request);
        })
        .RequireAuthorization()
        .WithTags("Authentication");
    }
}