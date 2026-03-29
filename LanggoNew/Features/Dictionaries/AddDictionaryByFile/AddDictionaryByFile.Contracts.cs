using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public class Command() : IRequest
{
    public string Name { get; set; } = null!;
    public string LangFrom { get; set; } = null!;
    public string LangTo { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPublic { get; set; }
    public List<DictionaryEntry> Entries { get; set; } = null!;
}