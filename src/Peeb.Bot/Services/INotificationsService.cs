using System.Collections.Generic;
using MediatR;

namespace Peeb.Bot.Services
{
    public interface INotificationsService
    {
        void AddNotification(INotification notification);
        T GetNotification<T>() where T : INotification;
        List<T> GetNotifications<T>() where T : INotification;
    }
}
