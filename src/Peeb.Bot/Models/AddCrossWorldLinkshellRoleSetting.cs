using System;

namespace Peeb.Bot.Models
{
    public class AddCrossWorldLinkshellRoleSetting : Setting
    {
        public string CrossWorldLinkshellId { get; private set; }
        public ulong RoleId { get; private set; }

        public AddCrossWorldLinkshellRoleSetting(ulong guildId, string crossWorldLinkshellId, ulong roleId, bool isEnabled, DateTimeOffset utcNow)
            : base(guildId, isEnabled, utcNow)
        {
            CrossWorldLinkshellId = crossWorldLinkshellId;
            RoleId = roleId;
        }

        private AddCrossWorldLinkshellRoleSetting()
        {
        }
    }
}
