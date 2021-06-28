using System.Collections.Generic;
using MediatR;

namespace Peeb.Bot.Models
{
    public abstract class Entity : INotificationSource
    {
        private readonly List<INotification> _notifications = new();

        IReadOnlyList<INotification> INotificationSource.Flush()
        {
            var notifications = new List<INotification>(_notifications);

            _notifications.Clear();

            return notifications;
        }

        protected void Publish(INotification notification)
        {
            _notifications.Add(notification);
        }
    }
}
