using System.Security.Cryptography;
using System.Text;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Authentication.Refresh;

public record RefreshTokenCommand(string Token) : IRequest<AuthResponse>;

public record AuthResponse(string AccessToken, string RefreshToken);

public class Handler(
    AppDbContext context,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = refreshTokenGenerator.HashToken(request.Token);
        
        var storedToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.IsRevoked || storedToken.User == null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        storedToken.IsRevoked = true;

        var newToken = refreshTokenGenerator.GenerateRefreshToken();
        var newTokenHash = refreshTokenGenerator.HashToken(newToken);

        var newTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            TokenHash = newTokenHash,
            Expires = DateTime.UtcNow.AddDays(30),
        };

        context.RefreshTokens.Add(newTokenEntity);
        await context.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenGenerator.GenerateJwtToken(storedToken.User);
        
        return new AuthResponse(accessToken, newToken);
    }
}