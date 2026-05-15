using MediatR;

namespace LanggoNew.Features.Games.JoinGame;

public record Command(string RoomId, string ConnectionId) : IRequest<Response>;
public record Response(List<UserData> Players);
public record UserData(
    int UserId,
    string Username,
    bool IsHost,
    string AvatarUrl,
    string NativeLanguage,
    int Rating);
    
    