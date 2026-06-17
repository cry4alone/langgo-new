using MediatR;

namespace LanggoNew.Features.User.SearchUserByUsername;

public record Query(string Username) : IRequest<List<UserSearchResult>>;

public sealed record UserSearchResult(
    int Id,
    string Username,
    string FullName,
    string? AvatarUrl,
    string LearningLanguage,
    string NativeLanguage
);
