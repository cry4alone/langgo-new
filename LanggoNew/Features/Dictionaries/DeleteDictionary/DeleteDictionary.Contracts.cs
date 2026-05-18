using MediatR;

namespace LanggoNew.Features.Dictionaries.DeleteDictionary;

public record Request(int DictionaryId) : IRequest;

