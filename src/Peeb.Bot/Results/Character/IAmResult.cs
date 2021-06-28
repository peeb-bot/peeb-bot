using Discord.Commands;
using Peeb.Bot.Dtos;

namespace Peeb.Bot.Results.Character
{
    public class IAmResult : RuntimeResult
    {
        public CharacterDto Character { get; }

        public IAmResult(CharacterDto character)
            : base(null, "OK")
        {
            Character = character;
        }
    }
}
