using MediatR;

namespace LanggoNew.Features.Games.CreateGame;

public record Command(
    string UserName,
    int DictionaryId, 
    int Mode, 
    int MaxRounds) : IRequest<Response>;

public record Response(string RoomId);