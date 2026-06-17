using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.GetPendingFriendRequests;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService,
    IAvatarStorageService avatarStorageService) : IRequestHandler<Query, List<PendingRequestResponse>>
{
    public async Task<List<PendingRequestResponse>> Handle(Query request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();

        var pendingRequests = await context.Friendships
            .Where(f => f.FriendId == currentUserId && f.Status == FriendshipStatus.Pending)
            .Select(f => new
            {
                FriendshipId = f.Id,
                UserId = f.User.Id,
                f.User.Username,
                f.User.FullName,
                f.User.Avatar
            })
            .ToListAsync(cancellationToken);

        var result = await Task.WhenAll(pendingRequests.Select(async r =>
        {
            string? avatarUrl = null;
            if (!string.IsNullOrWhiteSpace(r.Avatar) && r.Avatar != "string")
            {
                avatarUrl = await avatarStorageService.GetPresignedUrlAsync(r.Avatar, TimeSpan.FromMinutes(15));
            }

            return new PendingRequestResponse(
                r.FriendshipId,
                r.UserId,
                r.Username,
                r.FullName,
                avatarUrl);
        }));

        return result.ToList();
    }
}
