using System.Collections.Generic;
using MediatR;

namespace Peeb.Bot.Models
{
    public interface INotificationSource
    {
        IReadOnlyList<INotification> Flush();
    }
}
