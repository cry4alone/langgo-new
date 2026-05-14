using LanggoNew.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LanggoNew.Features.Authentication.Refresh;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh",
            async ([FromBody] RefreshTokenCommand command,
                ISender sender) =>
            {
                var response = await sender.Send(command);
                
                return TypedResults.Ok(response);
            })
            .WithTags("Authentication");
    }
}