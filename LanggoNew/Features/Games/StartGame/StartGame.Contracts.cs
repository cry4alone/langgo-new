using MediatR;

namespace LanggoNew.Features.Games.StartGame;

public record Command(string RoomId) : IRequest<Response>;
public record Response(DateTime GameStartTime);