using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.XivApi;
using Peeb.Bot.Data;
using Peeb.Bot.Notifications.RoleAdded;
using Peeb.Bot.Services;

namespace Peeb.Bot.Notifications.CharacterSaved
{
    public class AddCrossWorldLinkshellRoleHandler : INotificationHandler<CharacterSavedNotification>
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IMediator _mediator;
        private readonly IXivApiClient _xivApiClient;
        private readonly PeebDbContext _db;

        public AddCrossWorldLinkshellRoleHandler(
            IDateTimeOffsetService dateTimeOffsetService,
            IDiscordSocketClient discordSocketClient,
            IMediator mediator,
            IXivApiClient xivApiClient,
            PeebDbContext db)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
            _discordSocketClient = discordSocketClient;
            _mediator = mediator;
            _xivApiClient = xivApiClient;
            _db = db;
        }

        public async Task Handle(CharacterSavedNotification notification, CancellationToken cancellationToken)
        {
            var setting = await _db.AddCrossWorldLinkshellRoleSettings.SingleOrDefaultAsync(s => s.GuildId == notification.GuildId && s.IsEnabled, cancellationToken);

            if (setting == null)
            {
                return;
            }

            var crossWorldLinkshellResponse = await _xivApiClient.GetCrossWorldLinkshell(setting.CrossWorldLinkshellId);

            if (crossWorldLinkshellResponse == null)
            {
                setting.Disable(_dateTimeOffsetService.UtcNow);

                return;
            }

            var isMember = crossWorldLinkshellResponse.Linkshell.Results.Any(r => r.Id == notification.LodestoneId);

            if (isMember)
            {
                var guild = await _discordSocketClient.GetGuildAsync(notification.GuildId);
                var user = await guild.GetUserAsync(notification.UserId);

                await user.AddRoleAsync(setting.RoleId);
                await _mediator.Publish(new RoleAddedNotification(notification.UserId, setting.RoleId), cancellationToken);
            }
        }
    }
}
