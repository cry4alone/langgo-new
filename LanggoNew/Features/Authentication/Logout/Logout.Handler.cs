using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Authentication.Logout;

public record Command(string RefreshToken) : IRequest;

public class Handler(
    ICurrentUserService currentUserService,
    AppDbContext context,
    IRefreshTokenGenerator refreshTokenGenerator) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = int.Parse(currentUserService.GetCurrentUserId()); 
        
        var tokenHash = refreshTokenGenerator.HashToken(request.RefreshToken);

        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == currentUserId && rt.TokenHash == tokenHash, cancellationToken);

        if (refreshToken != null && !refreshToken.IsRevoked)
        {
            refreshToken.IsRevoked = true;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}