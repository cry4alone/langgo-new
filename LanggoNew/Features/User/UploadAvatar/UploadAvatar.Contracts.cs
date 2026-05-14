using MediatR;

namespace LanggoNew.Features.User.UploadAvatar;

public record Response(string AvatarKey);
public record Request(int UserId, IFormFile File) : IRequest<Response>;

