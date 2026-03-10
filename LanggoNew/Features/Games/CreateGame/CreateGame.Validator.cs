using FluentValidation;

namespace LanggoNew.Features.Games.CreateGame;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Mode).InclusiveBetween(0, 2);
        RuleFor(x => x.MaxRounds).GreaterThan(0).LessThan(30);
    }
}