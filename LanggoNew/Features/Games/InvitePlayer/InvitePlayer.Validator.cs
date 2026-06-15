using FluentValidation;

namespace LanggoNew.Features.Games.InvitePlayer;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.TargetUserId)
            .GreaterThan(0);

        RuleFor(x => x.RoomId)
            .NotEmpty();
    }
}
