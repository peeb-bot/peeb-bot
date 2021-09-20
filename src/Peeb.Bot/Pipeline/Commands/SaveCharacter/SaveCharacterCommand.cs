using MediatR;
using Peeb.Bot.Pipeline.Commands.SaveUser;

namespace Peeb.Bot.Pipeline.Commands.SaveCharacter
{
    public class SaveCharacterCommand : SaveUserCommand, IRequest<SaveCharacterCommandResult>
    {
        public ulong GuildId { get; }
        public string World { get; }
        public string Forename { get; }
        public string Surname { get; }

        public SaveCharacterCommand(ulong userId, ulong guildId, string world, string forename, string surname)
            : base(userId)
        {
            GuildId = guildId;
            World = world;
            Forename = forename;
            Surname = surname;
        }
    }
}
