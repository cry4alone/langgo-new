using FluentValidation;

namespace LanggoNew.Features.User.GetAvatar;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Avatar key is required");
    }
}

