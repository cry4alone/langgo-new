using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddWordsToDictionary;

public record Request(
    int DictionaryId,
    List<DictionaryEntry> WordsWithTranslations) : IRequest;

