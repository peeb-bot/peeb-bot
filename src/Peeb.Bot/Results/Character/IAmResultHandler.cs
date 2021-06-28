using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Helpers;
using Peeb.Bot.Notifications.NicknameChanged;
using Peeb.Bot.Notifications.RoleAdded;
using Peeb.Bot.Services;

namespace Peeb.Bot.Results.Character
{
    public class IAmResultHandler : ResultHandler<IAmResult>
    {
        private readonly INotificationsService _notificationsService;

        public IAmResultHandler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public override Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IAmResult result)
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(ColorHelper.Success)
                .WithTitle($"{result.Character.Forename} {result.Character.Surname} ({result.Character.World})")
                .WithThumbnailUrl(result.Character.AvatarUrl)
                .WithDescription("Character saved.");

            var nicknameChangedNotification = _notificationsService.GetNotification<NicknameChangedNotification>();

            if (nicknameChangedNotification != null)
            {
                embedBuilder.AddField("Nickname", $"Your Discord nickname was changed to **{nicknameChangedNotification.Nickname}**");
            }

            var roleAddedNotifications = _notificationsService.GetNotifications<RoleAddedNotification>();

            if (roleAddedNotifications.Any())
            {
                embedBuilder.AddField("Roles Added", roleAddedNotifications.Aggregate("", (v, n) => $"{v} <@&{n.RoleId}>").TrimStart());
            }

            return commandContext.Channel.SendMessageAsync(
                embed: embedBuilder.Build(),
                messageReference: new MessageReference(commandContext.Message.Id));
        }
    }
}
