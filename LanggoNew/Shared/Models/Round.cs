namespace LanggoNew.Models;

public class Round
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string CorrectWord { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    public Game Game { get; set; } = null!;
    public ICollection<RoundUser> RoundUsers { get; set; } = new List<RoundUser>();
}