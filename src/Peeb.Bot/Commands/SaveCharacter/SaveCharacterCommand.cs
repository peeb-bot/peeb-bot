using MediatR;
using Peeb.Bot.Commands.SaveUser;

namespace Peeb.Bot.Commands.SaveCharacter
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
