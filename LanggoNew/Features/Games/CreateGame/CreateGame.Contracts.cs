using MediatR;

namespace LanggoNew.Features.Games.CreateGame;

public record Command(
    int DictionaryId, 
    int Mode, 
    int MaxRounds) : IRequest<Response>;

public record Response(string RoomId);