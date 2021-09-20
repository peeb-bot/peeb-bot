using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Data;
using Peeb.Bot.Pipeline.Notifications.RoleAdded;

namespace Peeb.Bot.Pipeline.Notifications.CharacterSaved
{
    public class AddCharacterRoleHandler : INotificationHandler<CharacterSavedNotification>
    {
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IMediator _mediator;
        private readonly PeebDbContext _db;

        public AddCharacterRoleHandler(IDiscordSocketClient discordSocketClient, IMediator mediator, PeebDbContext db)
        {
            _discordSocketClient = discordSocketClient;
            _mediator = mediator;
            _db = db;
        }

        public async Task Handle(CharacterSavedNotification notification, CancellationToken cancellationToken)
        {
            var setting = await _db.AddCharacterRoleSettings.SingleOrDefaultAsync(s => s.GuildId == notification.GuildId && s.IsEnabled, cancellationToken);

            if (setting == null)
            {
                return;
            }

            var guild = await _discordSocketClient.GetGuildAsync(notification.GuildId);
            var user = await guild.GetUserAsync(notification.UserId);

            await user.AddRoleAsync(setting.RoleId);
            await _mediator.Publish(new RoleAddedNotification(notification.UserId, setting.RoleId), cancellationToken);
        }
    }
}
