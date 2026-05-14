using AutoMapper;
using LanggoNew.Shared.Models;

namespace LanggoNew.Features.Dictionaries.AddDictionary;

public class DictionaryProfile : Profile
{
    public DictionaryProfile()
    {
        CreateMap<Request, Dictionary>();
    }
}