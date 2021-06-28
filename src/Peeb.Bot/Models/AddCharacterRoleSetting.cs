using System;

namespace Peeb.Bot.Models
{
    public class AddCharacterRoleSetting : Setting
    {
        public ulong RoleId { get; private set; }

        public AddCharacterRoleSetting(ulong guildId, ulong roleId, bool isEnabled, DateTimeOffset utcNow)
            : base(guildId, isEnabled, utcNow)
        {
            RoleId = roleId;
        }

        private AddCharacterRoleSetting()
        {
        }
    }
}
