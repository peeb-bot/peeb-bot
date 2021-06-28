using System;

namespace Peeb.Bot.Models
{
    public class ChangeNicknameToCharacterNameSetting : Setting
    {
        public ChangeNicknameToCharacterNameSetting(ulong guildId, bool isEnabled, DateTimeOffset utcNow)
            : base(guildId, isEnabled, utcNow)
        {
        }

        private ChangeNicknameToCharacterNameSetting()
        {
        }
    }
}
