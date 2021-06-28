using AutoMapper;
using Peeb.Bot.Dtos;
using Peeb.Bot.Models;

namespace Peeb.Bot.Profiles
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            CreateMap<Character, CharacterDto>();
        }
    }
}
