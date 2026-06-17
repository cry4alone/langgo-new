using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.User.SearchUserByUsername;

public class Handler(AppDbContext context, IAvatarStorageService avatarStorageService) : IRequestHandler<Query, List<UserSearchResult>>
{
    public async Task<List<UserSearchResult>> Handle(Query request, CancellationToken cancellationToken)
    {
        var users = await context.Users
            .Where(u => u.Username.Contains(request.Username))
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.FullName,
                u.Avatar,
                u.LearningLanguage,
                u.NativeLanguage
            })
            .ToListAsync(cancellationToken);

        var result = await Task.WhenAll(users.Select(async u =>
        {
            string? avatarUrl = null;
            if (!string.IsNullOrWhiteSpace(u.Avatar) && u.Avatar != "string")
            {
                avatarUrl = await avatarStorageService.GetPresignedUrlAsync(u.Avatar, TimeSpan.FromMinutes(15));
            }

            return new UserSearchResult(
                u.Id,
                u.Username,
                u.FullName,
                avatarUrl,
                u.LearningLanguage,
                u.NativeLanguage);
        }));

        return result.ToList();
    }
}
