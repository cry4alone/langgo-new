namespace LanggoNew.Shared.Infrastructure.Services;


public interface IEmailVerificationLinkFactory
{
    string GenerateEmailVerificationLink(string emailVerificationToken);
}
public class EmailVerificationLinkFactory(
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator) : IEmailVerificationLinkFactory
{
    public string GenerateEmailVerificationLink(string emailVerificationToken)
    {
        var verificationlink = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            Features.Authentication.EmailVerify.Endpoint.EmailVerify,
            new
            {
                token = emailVerificationToken
            });
            
        return verificationlink ?? throw new Exception("Link generation failed");
    }
}