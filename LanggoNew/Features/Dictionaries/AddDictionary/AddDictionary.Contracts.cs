using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public record Request(
    string Name, 
    LanguageCode LangFrom, 
    LanguageCode LangTo,
    string Description,
    bool IsPublic,
    List<DictionaryEntry> WordsWithTranslations) : IRequest;

public record Response;

