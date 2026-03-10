using MediatR;

namespace LanggoNew.Features.Authentication.Register;

public record Command(
    string Email, 
    string Password,
    string Username,
    string? FullName,
    string? Avatar,
    string LearningLanguage,
    string NativeLanguage) : IRequest<Response>;

public record Response(
    string Token);
    