using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LanggoNew.Features.User.ChangeUserProfile;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("user/profile", async (Command request, ISender sender) =>
        {
            await sender.Send(request);
            return TypedResults.NoContent();
        })
        .RequireAuthorization()
        .WithTags("User");
    }
}