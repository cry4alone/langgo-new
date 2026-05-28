using FluentValidation;

namespace LanggoNew.Features.Friends.SendFriendRequest;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.FriendId).GreaterThan(0);
    }
}

