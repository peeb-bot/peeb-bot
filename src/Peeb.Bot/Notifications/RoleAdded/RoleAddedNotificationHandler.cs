using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Peeb.Bot.Services;

namespace Peeb.Bot.Notifications.RoleAdded
{
    public class RoleAddedNotificationHandler : INotificationHandler<RoleAddedNotification>
    {
        private readonly INotificationsService _notificationsService;

        public RoleAddedNotificationHandler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public Task Handle(RoleAddedNotification notification, CancellationToken cancellationToken)
        {
            _notificationsService.AddNotification(notification);

            return Task.CompletedTask;
        }
    }
}
