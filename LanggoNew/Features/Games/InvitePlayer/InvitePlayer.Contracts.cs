using MediatR;

namespace LanggoNew.Features.Games.InvitePlayer;

public record Command(string RoomId, int TargetUserId) : IRequest;
