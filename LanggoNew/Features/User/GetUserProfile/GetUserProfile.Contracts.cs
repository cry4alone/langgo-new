using MediatR;

namespace LanggoNew.Features.User.GetUserProfile;

public record Response(
    int Id,
    string Email,
    string Username,
    string FullName,
    string Avatar,
    string LearningLanguage,
    string NativeLanguage,
    int Rating); 
public record Request(int UserId) : IRequest<Response>;