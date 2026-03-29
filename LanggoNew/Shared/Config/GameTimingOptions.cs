namespace LanggoNew.Shared.Config;

public class GameTimingOptions
{
    public const string SectionName = "GameTiming";
    
    public int RoundDurationSeconds { get; init; } = 30;
    
    public int PauseBetweenRoundsSeconds { get; init; } = 5;
}