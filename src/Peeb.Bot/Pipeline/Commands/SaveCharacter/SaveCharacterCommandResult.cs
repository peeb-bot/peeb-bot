using Peeb.Bot.Dtos;

namespace Peeb.Bot.Pipeline.Commands.SaveCharacter
{
    public class SaveCharacterCommandResult
    {
        public CharacterDto Character { get; }

        public SaveCharacterCommandResult(CharacterDto character)
        {
            Character = character;
        }

        public SaveCharacterCommandResult()
        {
        }
    }
}
