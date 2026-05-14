using MediatR;

namespace LanggoNew.Features.User.GetAvatar;

public record Response(string Url);
public record Request(string Key) : IRequest<Response>;

