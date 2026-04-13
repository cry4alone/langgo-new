using LanggoNew.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Authentication.EmailVerify;
public record Command(string Token) : IRequest;

public class Handler(AppDbContext context) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var emailVerificationToken = await context.EmailVerificationTokens
            .Include(evt => evt.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token,
            cancellationToken: cancellationToken);

        if (emailVerificationToken is null)
        {
            throw new NotFoundException("Email verification token not found");
        }
        
        if (emailVerificationToken.User.IsEmailVerified)
        {
            return;
        }

        if (emailVerificationToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            throw new ConflictException("Email verification token has expired.");
        }

        
        emailVerificationToken.User.IsEmailVerified = true;
        
        context.Remove(emailVerificationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

