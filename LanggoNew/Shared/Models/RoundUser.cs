namespace LanggoNew.Models;

public class RoundUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoundId { get; set; }
    public int ResponseTimeMs { get; set; }
    
    public User User { get; set; } = null!;
    public Round Round { get; set; } = null!;
}