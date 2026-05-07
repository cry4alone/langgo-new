using LanggoNew.Models;

namespace LanggoNew.Shared.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public string TokenHash { get; set; } = string.Empty;
    public User User { get; set; }
}