using LanggoNew.Endpoints;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LanggoNew.Features.User.GetUserProfile;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/profile", async (ISender sender, ICurrentUserService currentUserService) =>
        {
            var userId = int.Parse(currentUserService.GetCurrentUserId());
            var request = new Request(userId);
            var response = await sender.Send(request);
            return TypedResults.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("User");
    }
}