using FluentValidation;

namespace LanggoNew.Features.User.UploadAvatar;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required")
            .Must(f => f.Length is > 0 and <= 5_000_000)
            .WithMessage("File must be between 1 byte and 5 MB");
    }
}

