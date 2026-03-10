using FluentValidation;

namespace LanggoNew.Features.Authentication.Register;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.LearningLanguage).NotEmpty();
        RuleFor(x => x.NativeLanguage).NotEmpty();
    }
}