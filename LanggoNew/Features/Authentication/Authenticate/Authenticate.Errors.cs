namespace LanggoNew.Features.Authentication.Authenticate;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException() : base("Invalid email or password.")
    {
    }
}