using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.User.ChangePassword;

public record Command(string CurrentPassword, string NewPassword) : IRequest;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService,
    IPasswordHashingService hashingService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();
        var user = await context.Users
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) 
                   ?? throw new NotFoundException(nameof(User));

        if (!hashingService.VerifyHashedPassword(user.Password, request.CurrentPassword))
            throw new InvalidCredentialsException();

        user.Password = hashingService.HashPassword(request.NewPassword);

        await context.SaveChangesAsync(cancellationToken);
    }
}