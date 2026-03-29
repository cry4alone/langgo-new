namespace LanggoNew.Shared.Models;

public record DictionaryEntry(
    string Original,
    string Translation,
    string Example,
    int Difficulty);