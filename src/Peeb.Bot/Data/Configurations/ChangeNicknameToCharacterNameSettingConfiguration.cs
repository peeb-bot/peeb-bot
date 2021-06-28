using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peeb.Bot.Models;

namespace Peeb.Bot.Data.Configurations
{
    public class ChangeNicknameToCharacterNameSettingConfiguration : IEntityTypeConfiguration<ChangeNicknameToCharacterNameSetting>
    {
        public void Configure(EntityTypeBuilder<ChangeNicknameToCharacterNameSetting> builder)
        {
            builder.HasBaseType<Setting>();
            builder.HasPartitionKey(s => s.GuildId);

            builder.HasData(
                new ChangeNicknameToCharacterNameSetting(
                    376090072627281920,
                    true,
                    DateTimeOffset.UtcNow));
        }
    }
}
