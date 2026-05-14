using LanggoNew.Shared.Enum;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public record Request(
    string Name, 
    LanguageCode LangFrom, 
    LanguageCode LangTo,
    string Description,
    bool IsPublic) : IRequest;

public record Response;

