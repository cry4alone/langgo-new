using FluentValidation;

namespace LanggoNew.Features.Authentication.Authenticate;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}