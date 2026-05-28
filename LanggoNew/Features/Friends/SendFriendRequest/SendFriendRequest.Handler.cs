using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.SendFriendRequest;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        if (request.FriendId == currentUserId)
            throw new ConflictException("Cannot send a friend request to yourself.");

        var targetExists = await context.Users
            .AnyAsync(u => u.Id == request.FriendId, cancellationToken);
        if (!targetExists)
            throw new NotFoundException("User");

        var relationshipExists = await context.Friendships.AnyAsync(
            f => (f.UserId == currentUserId && f.FriendId == request.FriendId) ||
                 (f.UserId == request.FriendId && f.FriendId == currentUserId),
            cancellationToken);
        if (relationshipExists)
            throw new ConflictException("Friendship or request already exists.");

        var friendship = new Friendship
        {
            UserId = currentUserId,
            FriendId = request.FriendId,
            Status = FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        context.Friendships.Add(friendship);
        await context.SaveChangesAsync(cancellationToken);
    }
}

