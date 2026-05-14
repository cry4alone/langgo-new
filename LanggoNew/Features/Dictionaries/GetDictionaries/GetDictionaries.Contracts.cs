using MediatR;

namespace LanggoNew.Features.Dictionaries.GetDictionaries;

public record DictionaryResponse(
    int Id, 
    string Name, 
    string LangFrom, 
    string LangTo, 
    string Description,
    int WordsCount);

public record Response(List<DictionaryResponse> Dictionaries);

public record Request : IRequest<Response>;

