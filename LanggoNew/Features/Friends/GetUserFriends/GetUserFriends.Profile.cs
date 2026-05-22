using AutoMapper;

namespace LanggoNew.Features.Friends.GetUserFriends;

public class GetUserFriendsProfile : Profile
{
    public GetUserFriendsProfile()
    {
        CreateMap<LanggoNew.Models.User, FriendResponse>()
            .ConstructUsing(src => new FriendResponse(
                src.Id,
                src.FullName,
                src.Avatar));
    }
}

