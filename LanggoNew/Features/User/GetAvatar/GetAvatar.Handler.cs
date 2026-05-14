using LanggoNew.Shared.Infrastructure.Services;
using MediatR;

namespace LanggoNew.Features.User.GetAvatar;

public class Handler(IAvatarStorageService avatarStorageService) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var presignedUrl = await avatarStorageService.GetPresignedUrlAsync(request.Key, TimeSpan.FromMinutes(15));
        return new Response(presignedUrl);
    }
}

