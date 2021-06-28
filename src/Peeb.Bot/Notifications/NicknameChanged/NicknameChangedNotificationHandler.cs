using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Peeb.Bot.Services;

namespace Peeb.Bot.Notifications.NicknameChanged
{
    public class NicknameChangedNotificationHandler : INotificationHandler<NicknameChangedNotification>
    {
        private readonly INotificationsService _notificationsService;

        public NicknameChangedNotificationHandler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public Task Handle(NicknameChangedNotification notification, CancellationToken cancellationToken)
        {
            _notificationsService.AddNotification(notification);

            return Task.CompletedTask;
        }
    }
}
