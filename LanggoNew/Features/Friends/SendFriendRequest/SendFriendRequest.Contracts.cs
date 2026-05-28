using MediatR;

namespace LanggoNew.Features.Friends.SendFriendRequest;

public record Command(int FriendId) : IRequest;

