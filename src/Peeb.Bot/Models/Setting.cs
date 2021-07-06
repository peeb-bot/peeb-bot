using System;

namespace Peeb.Bot.Models
{
    public abstract class Setting : Entity
    {
        public Guid Id { get; private set; }
        public ulong GuildId { get; private set; }
        public bool IsEnabled { get; private set; }
        public DateTimeOffset Created { get; private set; }
        public DateTimeOffset? Updated { get; protected set; }

        protected Setting(ulong guildId, bool isEnabled, DateTimeOffset utcNow)
        {
            Id = Guid.NewGuid();
            GuildId = guildId;
            IsEnabled = isEnabled;
            Created = utcNow;
        }

        protected Setting()
        {
        }

        public void Disable(DateTimeOffset utcNow)
        {
            IsEnabled = false;
            Updated = utcNow;
        }
    }
}
