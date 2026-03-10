using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LanggoNew.Features.Authentication.Authenticate;

public class Handler(AppDbContext context, IPasswordHashingService passwordHashingService, IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (user == null) throw new AuthenticationFailedException();
            
        var isPasswordValid = passwordHashingService.VerifyHashedPassword(user.Password, request.Password);
        if (!isPasswordValid) throw new AuthenticationFailedException();
             
        var token = jwtTokenGenerator.GenerateJwtToken(user);
             
        return new Response(token);
    }
}

