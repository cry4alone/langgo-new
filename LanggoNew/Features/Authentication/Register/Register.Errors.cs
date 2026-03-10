namespace LanggoNew.Features.Authentication.Register;

public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException() : base("The email address is already in use.")
    {
    }
}