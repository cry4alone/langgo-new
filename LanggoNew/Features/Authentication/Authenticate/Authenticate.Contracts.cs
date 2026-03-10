using MediatR;

namespace LanggoNew.Features.Authentication.Authenticate;

public record Command(string Email, string Password) : IRequest<Response>;
public record Response(string Token);