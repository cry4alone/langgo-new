using FluentValidation;

namespace LanggoNew.Features.Friends.RemoveFriend;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.FriendId).GreaterThan(0);
    }
}

