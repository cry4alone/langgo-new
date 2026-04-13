using LanggoNew.Shared.Models;

namespace LanggoNew.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } 
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string LearningLanguage { get; set; } = string.Empty;
    public string NativeLanguage { get; set; } = string.Empty;
    public int Rating { get; set; }
    
    public ICollection<Dictionary> Dictionaries { get; set; } = new List<Dictionary>();
    public ICollection<Game> WonGames { get; set; } = new List<Game>();
    public ICollection<RoundUser> RoundUsers { get; set; } = new List<RoundUser>();
}