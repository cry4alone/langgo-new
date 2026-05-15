using MediatR;

namespace LanggoNew.Features.Games.JoinGame;

public record Command(string RoomId) : IRequest<Response>;
public record Response(int UserId, 
    string Username,
    bool IsHost,
    string AvatarUrl,
    string NativeLanguage,
    int Rating);
    
    