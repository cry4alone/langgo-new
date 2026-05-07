using FluentEmail.Core;
using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Authentication.Register;

public class Handler(
    AppDbContext context,
    IPasswordHashingService passwordHashingService,
    IJwtTokenGenerator jwtTokenGenerator,
    IFluentEmail fluentEmail,
    IEmailVerificationLinkFactory emailVerificationLinkFactory,
    IRefreshTokenGenerator refreshTokenGenerator) : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (existingUser is not null)
            throw new EmailAlreadyInUseException();
        
        var hashedPassword = passwordHashingService.HashPassword(request.Password);
        
        var user = new Models.User
        {
             Email = request.Email,
             Password = hashedPassword, 
             Username = request.Username,
             FullName = request.FullName ?? string.Empty,
             Avatar = request.Avatar ?? string.Empty,
             LearningLanguage = request.LearningLanguage,
             NativeLanguage = request.NativeLanguage
        };
         
        context.Users.Add(user);


        var verificationToken = GenerateVerificationToken();
        
        context.EmailVerificationTokens.Add(new EmailVerificationToken
        {
            Token = verificationToken,
            User = user,
            CreatedOnUtc = DateTime.UtcNow,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(1),
        });
        
        var verificationLink = emailVerificationLinkFactory.GenerateEmailVerificationLink(verificationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        await fluentEmail
            .To(user.Email)
            .Subject("Email verification for Langgo")
            .Body($"To verify your email address <a href={verificationLink}>click here</a>", isHtml: true)
            .SendAsync();
        
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

    private static string GenerateVerificationToken()
    {
        return Guid.NewGuid().ToString();
    }
    
}