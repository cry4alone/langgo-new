using MediatR;

namespace LanggoNew.Features.Friends.GetPendingFriendRequests;

public record Query() : IRequest<List<PendingRequestResponse>>;

public sealed record PendingRequestResponse(
    int FriendshipId,
    int UserId,
    string Username,
    string FullName,
    string? AvatarUrl
);
