using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.RespondToFriendRequest;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();

        var friendship = await context.Friendships
            .FirstOrDefaultAsync(
                f => f.UserId == request.RequesterId && f.FriendId == currentUserId,
                cancellationToken);

        if (friendship is null)
            throw new NotFoundException("Friend request");

        if (friendship.Status != FriendshipStatus.Pending)
            throw new ConflictException("Friend request already handled.");

        if (request.Accept)
        {
            friendship.Status = FriendshipStatus.Accepted;
        }
        else
        {
            context.Friendships.Remove(friendship);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

