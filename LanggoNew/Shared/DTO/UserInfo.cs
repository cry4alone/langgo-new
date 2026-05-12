namespace LanggoNew.Shared.DTO;

public record UserInfo(
    int Id,
    string Email,
    string Username,
    string FullName,
    string Avatar,
    string LearningLanguage,
    string NativeLanguage,
    int Rating);