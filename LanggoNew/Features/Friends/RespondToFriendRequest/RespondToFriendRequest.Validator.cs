using FluentValidation;

namespace LanggoNew.Features.Friends.RespondToFriendRequest;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.RequesterId).GreaterThan(0);
    }
}

