using MediatR;

namespace LanggoNew.Features.User.GetUserById;

public record Query(int UserId) : IRequest<UserProfile?>;

public sealed class UserProfile
{
    private readonly int _id;
    private readonly string _username;
    private readonly string _fullName;
    private readonly string? _avatarUrl;
    private readonly string _learningLanguage;
    private readonly string _nativeLanguage;

    public int Id => _id;
    public string Username => _username;
    public string FullName => _fullName;
    public string? AvatarUrl => _avatarUrl;
    public string LearningLanguage => _learningLanguage;
    public string NativeLanguage => _nativeLanguage;

    public UserProfile(
        int id,
        string username,
        string fullName,
        string? avatarUrl,
        string learningLanguage,
        string nativeLanguage)
    {
        _id = id;
        _username = username;
        _fullName = fullName;
        _avatarUrl = avatarUrl;
        _learningLanguage = learningLanguage;
        _nativeLanguage = nativeLanguage;
    }
}
