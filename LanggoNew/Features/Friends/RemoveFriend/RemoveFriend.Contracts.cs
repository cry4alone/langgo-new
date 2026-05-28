using MediatR;

namespace LanggoNew.Features.Friends.RemoveFriend;

public record Command(int FriendId) : IRequest;

