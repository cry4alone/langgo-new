using FluentEmail.Core;
using LanggoNew;
using LanggoNew.Models;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CancellationToken = System.Threading.CancellationToken;
using Task = System.Threading.Tasks.Task;

public record Command(
    string? Email,
    string? Username,
    string? FullName,
    string? Avatar,
    string? LearningLanguage,
    string? NativeLanguage) : IRequest;

public class Handler(
    ICurrentUserService currentUserService,
    AppDbContext context,
    IFluentEmail fluentEmail,
    IEmailVerificationLinkFactory emailVerificationLinkFactory) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User));

        if (request.Username is not null && request.Username != user.Username)
        {
            var exists = await context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (exists) throw new ConflictException("Username is already taken");
            
            user.Username = request.Username;
        }

        if (request.Email is not null && request.Email != user.Email)
        {
            var exists = await context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (exists) throw new ConflictException("Email is already registered");

            var newEmail = request.Email;
            user.Email = newEmail;
            user.IsEmailVerified = false;

            var verificationToken = emailVerificationLinkFactory.GenerateVerificationToken();
            
            context.EmailVerificationTokens.Add(new EmailVerificationToken
            {
                Token = verificationToken,
                UserId = user.Id, 
                CreatedOnUtc = DateTime.UtcNow,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(1),
            });

            var verificationLink = emailVerificationLinkFactory.GenerateEmailVerificationLink(verificationToken);

            await fluentEmail
                .To(newEmail)
                .Subject("Verify your new email for Langgo")
                .Body($"Click here to verify: <a href=\"{verificationLink}\">Verify Email</a>", isHtml: true)
                .SendAsync();
        }

        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.Avatar is not null) user.Avatar = request.Avatar;
        if (request.LearningLanguage is not null) user.LearningLanguage = request.LearningLanguage;
        if (request.NativeLanguage is not null) user.NativeLanguage = request.NativeLanguage;

        await context.SaveChangesAsync(cancellationToken);
    }
    
}