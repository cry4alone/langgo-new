using AutoMapper;
using LanggoNew.Shared.Models;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public class DictionaryProfile : Profile
{
    public DictionaryProfile()
    {
        CreateMap<Request, Dictionary>()
            .ForMember(d => d.LangFrom, opt => opt.MapFrom(s => s.LangFrom.ToString().ToLowerInvariant()))
            .ForMember(d => d.LangTo, opt => opt.MapFrom(s => s.LangTo.ToString().ToLowerInvariant()));
    }
}