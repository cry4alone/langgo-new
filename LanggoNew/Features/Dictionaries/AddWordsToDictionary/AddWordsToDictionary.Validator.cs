using FluentValidation;
using LanggoNew.Shared.Validators;

namespace LanggoNew.Features.Dictionaries.AddWordsToDictionary;

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.DictionaryId).GreaterThan(0);
        RuleFor(r => r.WordsWithTranslations)
            .NotEmpty()
            .Must(r => r.Count <= 100)
            .WithMessage("You can add up to 100 words at a time.");
        RuleForEach(r => r.WordsWithTranslations)
            .SetValidator(new DictionaryEntryValidator());
    }
}
