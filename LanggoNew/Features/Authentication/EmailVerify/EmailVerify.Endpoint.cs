using LanggoNew.Endpoints;
using LanggoNew.Shared.Exceptions;
using MediatR;

namespace LanggoNew.Features.Authentication.EmailVerify;

public class Endpoint : IEndpoint
{
    public const string EmailVerify = "EmailVerify";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/email-verify", async (string token, ISender sender, IConfiguration configuration, CancellationToken cancellationToken) =>
            {
                var frontendUrl = configuration["Frontend:Url"] ?? "http://localhost:5173";
                
                try
                {
                    await sender.Send(new Command(token), cancellationToken);
                    return Results.Redirect($"{frontendUrl}/email-verified?status=success");
                }
                catch (NotFoundException)
                {
                    return Results.Redirect($"{frontendUrl}/email-verified?status=error&reason=invalid_token");
                }
                catch (ConflictException)
                {
                    return Results.Redirect($"{frontendUrl}/email-verified?status=error&reason=token_expired");
                }
            })
            .WithTags("Authentication")
            .WithName(EmailVerify);
    }
}