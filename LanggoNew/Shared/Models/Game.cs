namespace LanggoNew.Models;

public class Game
{
    public int Id { get; set; }
    public int DictionaryId { get; set; }
    public string? Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? WinnerId { get; set; }
    public int RoundsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public Dictionary Dictionary { get; set; } = null!;
    public User? Winner { get; set; }
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public ICollection<RoundUser> RoundUsers { get; set; } = new List<RoundUser>();
}