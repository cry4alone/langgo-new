using FluentValidation;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.LangFrom).NotEmpty();
        RuleFor(r => r.LangTo).NotEmpty();
    }
}

