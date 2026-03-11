using MediatR;

namespace LanggoNew.Features.Games.JoinGame;

public record Command(string RoomId, int UserId) : IRequest;