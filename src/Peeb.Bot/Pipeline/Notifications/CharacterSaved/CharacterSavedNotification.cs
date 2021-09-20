using System;
using MediatR;

namespace Peeb.Bot.Pipeline.Notifications.CharacterSaved
{
    public class CharacterSavedNotification : INotification
    {
        public Guid Id { get; }
        public ulong UserId { get; }
        public ulong GuildId { get; }
        public int LodestoneId { get; }
        public string World { get; }
        public string Forename { get; }
        public string Surname { get; }
        public string Name { get; }
        public string AvatarUrl { get; }
        public DateTimeOffset Created { get; }
        public DateTimeOffset? Updated { get; }

        public CharacterSavedNotification(
            Guid id,
            ulong userId,
            ulong guildId,
            int lodestoneId,
            string world,
            string forename,
            string surname,
            string avatarUrl,
            DateTimeOffset created,
            DateTimeOffset? updated)
        {
            Id = id;
            UserId = userId;
            GuildId = guildId;
            LodestoneId = lodestoneId;
            World = world;
            Forename = forename;
            Surname = surname;
            Name = $"{Forename} {Surname}";
            AvatarUrl = avatarUrl;
            Created = created;
            Updated = updated;
        }
    }
}
