using LanggoNew.Endpoints;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LanggoNew.Features.User.UploadAvatar;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user/{userId}/avatar",
                async Task<IResult> (string userId,
                    IFormFile file,
                    ICurrentUserService currentUserService,
                    ISender sender) =>
                {
                    if (!int.TryParse(userId, out var requestedUserId)) 
                        return TypedResults.BadRequest("Invalid user id");

                    var currentId = currentUserService.GetCurrentUserId();
                    if (currentId != requestedUserId) 
                        return TypedResults.Forbid();

                    var request = new Request(requestedUserId, file);
                    var response = await sender.Send(request);

                    return TypedResults.Ok(response);
                })
        .RequireAuthorization()
        .WithTags("User")
        .DisableAntiforgery();
    }
}