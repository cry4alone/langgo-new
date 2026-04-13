using LanggoNew.Models;

namespace LanggoNew.Shared.Models;

public class EmailVerificationToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public User User { get; set; }
}