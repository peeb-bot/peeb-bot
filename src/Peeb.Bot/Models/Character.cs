using System;
using Peeb.Bot.Notifications.CharacterSaved;

namespace Peeb.Bot.Models
{
    public class Character : Entity
    {
        public Guid Id { get; private set; }
        public User User { get; private set; }
        public ulong UserId { get; private set; }
        public ulong GuildId { get; private set; }
        public int LodestoneId { get; private set; }
        public string World { get; private set; }
        public string Forename { get; private set; }
        public string Surname { get; private set; }
        public string AvatarUrl { get; private set; }
        public DateTimeOffset Created { get; private set; }
        public DateTimeOffset? Updated { get; private set; }

        public Character(User user, ulong guildId, int lodestoneId, string world, string forename, string surname, string avatarUrl, DateTimeOffset utcNow)
        {
            Id = Guid.NewGuid();
            User = user;
            UserId = user.Id;
            GuildId = guildId;
            LodestoneId = lodestoneId;
            World = world;
            Forename = forename;
            Surname = surname;
            AvatarUrl = avatarUrl;
            Created = utcNow;
            Updated = null;

            Publish(new CharacterSavedNotification(Id, UserId, GuildId, LodestoneId, World, Forename, Surname, AvatarUrl, Created, Updated));
        }

        private Character()
        {
        }

        public void Update(int lodestoneId, string world, string forename, string surname, string avatarUrl, DateTimeOffset utcNow)
        {
            LodestoneId = lodestoneId;
            World = world;
            Forename = forename;
            Surname = surname;
            AvatarUrl = avatarUrl;
            Updated = utcNow;

            Publish(new CharacterSavedNotification(Id, UserId, GuildId, LodestoneId, World, Forename, Surname, AvatarUrl, Created, Updated));
        }
    }
}
