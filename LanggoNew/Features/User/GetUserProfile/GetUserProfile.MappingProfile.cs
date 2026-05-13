using AutoMapper;

namespace LanggoNew.Features.User.GetUserProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Models.User, Response>()
            .ConstructUsing(src => new Response(
                src.Id,
                src.Email,
                src.Username,
                src.FullName,
                src.Avatar,
                src.LearningLanguage,
                src.NativeLanguage,
                src.Rating
            ));
    }
}