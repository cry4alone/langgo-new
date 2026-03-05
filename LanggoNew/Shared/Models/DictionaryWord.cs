namespace LanggoNew.Models;

public class DictionaryWord
{
    public int Id { get; set; }
    public int DictionaryId { get; set; }
    public string Original { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public int Difficulty { get; set; }

    public Dictionary Dictionary { get; set; } = null!;
}