using MediatR;

namespace LanggoNew.Features.User.GetUserDictionaries;

public record DictionaryResponse(
    int Id, 
    string Name, 
    string LangFrom, 
    string LangTo, 
    string Description,
    int WordsCount);

public record Response(List<DictionaryResponse> Dictionaries);

public record Request(int UserId) : IRequest<Response>;
