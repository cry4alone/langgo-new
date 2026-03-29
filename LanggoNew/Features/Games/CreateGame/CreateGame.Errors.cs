namespace LanggoNew.Features.Games.CreateGame;

public class DictionaryNotFoundException : Exception
{
    public DictionaryNotFoundException(int dictionaryId) : base($"Dictionary with ID {dictionaryId} not found.")
    {
    }
}

public class NotEnoughWordsInDictionaryException : Exception
{
    public NotEnoughWordsInDictionaryException(int dictionaryId, int requiredWords) 
        : base($"Dictionary with ID {dictionaryId} does not contain enough words. Required: {requiredWords}.")
    {
    }
}