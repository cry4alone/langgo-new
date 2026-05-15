using AutoMapper;

namespace LanggoNew.Features.Games.JoinGame;

public class JoinGameProfile : Profile
{
    public JoinGameProfile()
    {
        CreateMap<LanggoNew.Models.User, UserData>()
            .ConstructUsing((user, context) => new UserData(
                user.Id,
                user.Username,
                false,
                user.Avatar,
                user.NativeLanguage,
                user.Rating));
    }
}





