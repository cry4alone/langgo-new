using MediatR;

namespace LanggoNew.Features.Friends.RespondToFriendRequest;

public record Request(bool Accept);

public record Command(int RequesterId, bool Accept) : IRequest;

