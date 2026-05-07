using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LanggoNew.Features.Authentication.Authenticate;

public class Handler(
    AppDbContext context,
    IPasswordHashingService passwordHashingService,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator)
    : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (user == null) throw new AuthenticationFailedException();
            
        var isPasswordValid = passwordHashingService.VerifyHashedPassword(user.Password, request.Password);
        if (!isPasswordValid) throw new AuthenticationFailedException();
             
        var accessToken = jwtTokenGenerator.GenerateJwtToken(user);
        var refreshToken = refreshTokenGenerator.GenerateRefreshToken();
        
        context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenGenerator.HashToken(refreshToken),
            Expires = DateTime.UtcNow.AddDays(30),
        });
        
        await context.SaveChangesAsync(cancellationToken);
        
        return new Response(accessToken, refreshToken);
    }
}

