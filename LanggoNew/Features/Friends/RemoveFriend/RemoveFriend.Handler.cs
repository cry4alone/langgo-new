using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.RemoveFriend;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();

        var friendship = await context.Friendships
            .FirstOrDefaultAsync(
                f => ((f.UserId == currentUserId && f.FriendId == request.FriendId) ||
                      (f.UserId == request.FriendId && f.FriendId == currentUserId)),
                cancellationToken);

        if (friendship is null)
            throw new NotFoundException("Friendship");

        if (friendship.Status != FriendshipStatus.Accepted)
            throw new ConflictException("Friendship is not accepted.");

        context.Friendships.Remove(friendship);
        await context.SaveChangesAsync(cancellationToken);
    }
}

