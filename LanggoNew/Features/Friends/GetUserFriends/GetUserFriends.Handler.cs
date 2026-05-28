using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.GetUserFriends;

public record Query(int UserId) : IRequest<List<FriendResponse>>;

public record FriendResponse(
    int UserId,
    string FullName,
    string AvatarUrl);

public class Handler(AppDbContext context) : IRequestHandler<Query, List<FriendResponse>>
{
    public async Task<List<FriendResponse>> Handle(Query request, CancellationToken cancellationToken)
    {
        var currUser = context.Users.FirstOrDefault(u => u.Id == request.UserId);
        if (currUser is null) 
            throw new KeyNotFoundException("User not found");
        
        var friendsFromRequests = context.Friendships
            .Where(f => f.UserId == request.UserId && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.Friend);

        var friendsFromIncoming = context.Friendships
            .Where(f => f.FriendId == request.UserId && f.Status == FriendshipStatus.Accepted)
            .Select(f => f.User);

        var friends = await friendsFromRequests
            .Union(friendsFromIncoming)
            .Select(u => new FriendResponse(
                u.Id,
                u.FullName,
                u.Avatar))
            .ToListAsync(cancellationToken);
        
        return friends;
    }
}
