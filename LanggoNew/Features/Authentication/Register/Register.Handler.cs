using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Authentication.Register;

public class Handler(AppDbContext context, IPasswordHashingService passwordHashingService, IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<Command, Response>
{
    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (existingUser is not null)
            throw new EmailAlreadyInUseException();
        
        var hashedPassword = passwordHashingService.HashPassword(request.Password);
        
        var user = new User
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
        await context.SaveChangesAsync(cancellationToken);
        
        var token = jwtTokenGenerator.GenerateJwtToken(user);
        return new Response(token);
    }
}