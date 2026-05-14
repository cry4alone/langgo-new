using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public record Request(
    string Name,
    string LangFrom,
    string LangTo,
    string Description,
    bool IsPublic,
    List<DictionaryEntry> Entries) : IRequest;
