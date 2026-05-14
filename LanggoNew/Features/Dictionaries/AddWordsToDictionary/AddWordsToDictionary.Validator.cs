using FluentValidation;
using LanggoNew.Shared.Models;

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
        RuleForEach(r => r.WordsWithTranslations).SetValidator(new DictionaryEntryValidator());
    }
}

public class DictionaryEntryValidator : AbstractValidator<DictionaryEntry>
{
    public DictionaryEntryValidator()
    {
        RuleFor(e => e.Original).NotEmpty();
        RuleFor(e => e.Translation).NotEmpty();
        RuleFor(e => e.Difficulty).InclusiveBetween(1, 5);
    }
}

