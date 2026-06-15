using FluentValidation;

namespace LanggoNew.Features.Games.RespondToInvite;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty();
    }
}
