namespace LanggoNew.Models;

public class Dictionary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LangFrom { get; set; } = string.Empty;
    public string LangTo { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public IEnumerable<DictionaryWord> Words { get; set; } = new List<DictionaryWord>();
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
