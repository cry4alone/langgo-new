using System.Security.Claims;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface ICurrentUserService
{
    string GetCurrentUserId();  
}

public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public string GetCurrentUserId()
    {
        var context = contextAccessor.HttpContext;
        if (context is null)
            return "";
        var currentUserId = context.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        
        return currentUserId;
    }
}