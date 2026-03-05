using AutoMapper;
using LanggoNew.Models;

namespace LanggoNew.Features.Dictionaries;

public class DictionaryProfile : Profile
{
    public DictionaryProfile()
    {
        CreateMap<AddDictionary.AddDictionary.Request, Dictionary>();
    }
}