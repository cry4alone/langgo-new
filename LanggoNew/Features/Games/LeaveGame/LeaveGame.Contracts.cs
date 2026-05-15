using MediatR;

namespace LanggoNew.Features.Games.LeaveGame;

public record Command(string RoomId) : IRequest<Response>;
public record Response(int UserId);