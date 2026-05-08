using System.Security.Claims;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface ICurrentUserService
{
    int GetCurrentUserId();  
}

public class CurrentUserService(IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public int GetCurrentUserId()
    {
        var context = contextAccessor.HttpContext;
        if (context == null) 
            throw new UnauthorizedAccessException();
        
        var currentUserId = context.User.Claims
            .First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            
        return int.Parse(currentUserId);
    }
}