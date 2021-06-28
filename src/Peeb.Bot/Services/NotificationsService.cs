using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace Peeb.Bot.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly List<INotification> _notifications = new();

        public void AddNotification(INotification notification)
        {
            _notifications.Add(notification);
        }

        public T GetNotification<T>() where T : INotification
        {
            return _notifications.OfType<T>().SingleOrDefault();
        }

        public List<T> GetNotifications<T>() where T : INotification
        {
            return _notifications.OfType<T>().ToList();
        }
    }
}
