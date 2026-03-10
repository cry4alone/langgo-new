namespace LanggoNew.Features.Games.CreateGame;

public class DictionaryNotFoundException : Exception
{
    public DictionaryNotFoundException(int dictionaryId) : base($"Dictionary with ID {dictionaryId} not found.")
    {
    }
}