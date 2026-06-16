using LanggoNew.Shared.Enum;

namespace LanggoNew.Shared.Models;

public class GameState
{
    public string RoomId { get; set; } = string.Empty;
    public int GameId { get; set; }
    public int DictionaryId { get; set; }
    public string HostUserId { get; set; } = string.Empty;
    public List<int> PlayerUserIds { get; set; } = [];
    public int MaxRounds { get; set; } = 15;
    public GameStatus Status { get; set; } = GameStatus.Waiting;
    public int CurrentRound { get; set; } = 0;
    public WordData CurrentWordData { get; set; } 
    public List<WordData> GameWords { get; set; } = [];

    public string? CurrentJobId { get; set; } 
    
    public DateTime? CurrentJobEndTimeUtc { get; set; }

    public bool IsRoundEnding { get; set; }

    public bool IsCurrentRoundChoice { get; set; }
    public List<string> CurrentRoundOptions { get; set; } = [];
    public int CurrentCorrectOptionIndex { get; set; }

    public int LastEndedRound { get; set; }

    public Stack<int> RoundWinners { get; set; } = new Stack<int>();
    public Dictionary<int, int> UserScores { get; set; } = new Dictionary<int, int>();
    public HashSet<int> PendingInvites { get; set; } = [];
}

public class WordData
{
    public int DictionaryWordId { get; set; }
    public string Original { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
}