using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Friends.GetUserFriends;

public record Query(int UserId) : IRequest<List<UserProfile>>;

public sealed class UserProfile
{
    private readonly int _id;
    private readonly string _username;
    private readonly string _fullName;
    private readonly string? _avatarUrl;
    private readonly string _learningLanguage;
    private readonly string _nativeLanguage;

    public int Id => _id;
    public string Username => _username;
    public string FullName => _fullName;
    public string? AvatarUrl => _avatarUrl;
    public string LearningLanguage => _learningLanguage;
    public string NativeLanguage => _nativeLanguage;

    public UserProfile(
        int id,
        string username,
        string fullName,
        string? avatarUrl,
        string learningLanguage,
        string nativeLanguage)
    {
        _id = id;
        _username = username;
        _fullName = fullName;
        _avatarUrl = avatarUrl;
        _learningLanguage = learningLanguage;
        _nativeLanguage = nativeLanguage;
    }
}

public class Handler(AppDbContext context, IAvatarStorageService avatarStorageService) : IRequestHandler<Query, List<UserProfile>>
{
    public async Task<List<UserProfile>> Handle(Query request, CancellationToken cancellationToken)
    {
        var currUserExists = await context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!currUserExists)
            throw new KeyNotFoundException("User not found");
        
        var friendsFromRequests = context.Friendships
            .Where(f => f.UserId == request.UserId && f.Status == FriendshipStatus.Accepted)
            .Select(f => new
            {
                f.Friend.Id,
                f.Friend.Username,
                f.Friend.FullName,
                f.Friend.Avatar,
                f.Friend.LearningLanguage,
                f.Friend.NativeLanguage
            });

        var friendsFromIncoming = context.Friendships
            .Where(f => f.FriendId == request.UserId && f.Status == FriendshipStatus.Accepted)
            .Select(f => new
            {
                f.User.Id,
                f.User.Username,
                f.User.FullName,
                f.User.Avatar,
                f.User.LearningLanguage,
                f.User.NativeLanguage
            });

        var friends = await friendsFromRequests
            .Union(friendsFromIncoming)
            .ToListAsync(cancellationToken);

        var result = await Task.WhenAll(friends.Select(async friend =>
        {
            string? avatarUrl = null;
            if (!string.IsNullOrWhiteSpace(friend.Avatar) || friend.Avatar != "string")
            {
                avatarUrl = await avatarStorageService.GetPresignedUrlAsync(friend.Avatar, TimeSpan.FromMinutes(15));
            }

            return new UserProfile(
                friend.Id,
                friend.Username,
                friend.FullName,
                avatarUrl,
                friend.LearningLanguage,
                friend.NativeLanguage);
        }));

        return result.ToList();
    }
}
