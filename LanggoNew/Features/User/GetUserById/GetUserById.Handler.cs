using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.User.GetUserById;

public class Handler(AppDbContext context, IAvatarStorageService avatarStorageService) : IRequestHandler<Query, UserProfile?>
{
    public async Task<UserProfile?> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return null;

        string? avatarUrl = null;
        if (!string.IsNullOrWhiteSpace(user.Avatar) && user.Avatar != "string")
        {
            avatarUrl = await avatarStorageService.GetPresignedUrlAsync(user.Avatar, TimeSpan.FromMinutes(15));
        }

        return new UserProfile(
            user.Id,
            user.Username,
            user.FullName,
            avatarUrl,
            user.LearningLanguage,
            user.NativeLanguage);
    }
}
