namespace LanggoNew.Shared.Exceptions;

public class NotFoundException(string entityName) : Exception($"{entityName} was not found.");