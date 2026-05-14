using LanggoNew.Shared.Infrastructure.Services;
using MediatR;

namespace LanggoNew.Features.User.UploadAvatar;

public class Handler(AppDbContext context, IAvatarStorageService avatarStorage) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([request.UserId], cancellationToken: cancellationToken)
                   ?? throw new KeyNotFoundException("User not found");

        var oldAvatarKey = user.Avatar;

        var key = await avatarStorage.UploadAsync(request.File, request.UserId.ToString());

        user.Avatar = key;
        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(oldAvatarKey))
        {
            await avatarStorage.DeleteAsync(oldAvatarKey);
        }

        return new Response(key);
    }
}
