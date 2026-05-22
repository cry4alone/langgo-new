using LanggoNew.Models;
using LanggoNew.Shared.Enum;

namespace LanggoNew.Shared.Models;

public class Friendship
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public FriendshipStatus Status { get; set; } 
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; } = null!;
    public User Friend { get; set; } = null!;
}