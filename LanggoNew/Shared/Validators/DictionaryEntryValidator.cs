using FluentValidation;
using LanggoNew.Shared.Models;

namespace LanggoNew.Shared.Validators;

public class DictionaryEntryValidator : AbstractValidator<DictionaryEntry>
{
    public DictionaryEntryValidator()
    {
        RuleFor(e => e.Original).NotEmpty();
        RuleFor(e => e.Translation).NotEmpty();
        RuleFor(e => e.Difficulty).InclusiveBetween(1, 5);
    }
}

