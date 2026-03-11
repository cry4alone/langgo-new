using MediatR;

namespace LanggoNew.Features.Games.LeaveGame;

public record Command(int UserId, string RoomId) : IRequest;