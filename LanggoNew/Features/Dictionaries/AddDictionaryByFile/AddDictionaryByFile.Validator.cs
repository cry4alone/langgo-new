using FluentValidation;
using LanggoNew.Shared.Validators;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.LangFrom).NotEmpty();
        RuleFor(r => r.LangTo).NotEmpty();
        RuleFor(r => r.Entries)
            .NotEmpty()
            .Must(r => r.Count <= 100)
            .WithMessage("You can add up to 100 words at a time.");
        RuleForEach(r => r.Entries).SetValidator(new DictionaryEntryValidator());
    }
}
