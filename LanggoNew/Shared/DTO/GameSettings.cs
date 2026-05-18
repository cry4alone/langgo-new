using LanggoNew.Shared.Enum;

namespace LanggoNew.Shared.DTO;

public record GameSettings(string DictionaryName,
    LanguageCode LangFrom,
    LanguageCode LangTo,
    int RoundsAmount);