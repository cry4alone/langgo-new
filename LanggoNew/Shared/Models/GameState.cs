using LanggoNew.Shared.Enum;

namespace LanggoNew.Shared.Models;

public class GameState
{
    public string RoomId { get; set; } = string.Empty;
    public int GameId { get; set; }
    public string HostUserId { get; set; } = string.Empty;

    public List<int> PlayerUserIds { get; set; } = new List<int>();
    public int MaxRounds { get; set; } = 15;
    public GameStatus Status { get; set; } = GameStatus.Waiting;

    public int CurrentRound { get; set; } = 0;
    public string CurrentWord { get; set; } = string.Empty;
    public string CurrentCorrectAnswer { get; set; } = string.Empty;
    
    public Dictionary<string, int> UserScores { get; set; } = new Dictionary<string, int>();
    //public HashSet<string> UsersAnsweredThisRound { get; set; } = []; если хотим отслеживать, кто уже ответил в текущем раунде, чтобы не допустить повторных ответов
}