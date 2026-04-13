using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Authentication.EmailVerify;

public class Endpoint : IEndpoint
{
    public const string EmailVerify = "EmailVerify";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/email-verify", async (string token, ISender sender, CancellationToken cancellationToken) =>
            {
                await sender.Send(new Command(token), cancellationToken);
                
                return Results.Ok();
            })
            .WithTags("Authentication")
            .WithName(EmailVerify);
    }
}