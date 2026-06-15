using MediatR;

namespace LanggoNew.Features.Games.RespondToInvite;

public record Command(string RoomId, bool Accept) : IRequest;
