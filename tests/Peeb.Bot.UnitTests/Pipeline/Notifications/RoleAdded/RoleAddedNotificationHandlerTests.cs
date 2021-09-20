using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Pipeline.Notifications.RoleAdded;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Pipeline.Notifications.RoleAdded
{
    [TestFixture]
    [Parallelizable]
    public class RoleAddedNotificationHandlerTests : TestBase<RoleAddedNotificationHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldAddNotification()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.NotificationsService.Verify(s => s.AddNotification(c.Notification), Times.Once));
        }
    }

    public class RoleAddedNotificationHandlerTestsContext
    {
        public RoleAddedNotificationHandler Handler { get; set; }
        public RoleAddedNotification Notification { get; set; }
        public Mock<INotificationsService> NotificationsService { get; set; }

        public RoleAddedNotificationHandlerTestsContext()
        {
            Notification = new RoleAddedNotification(1, 2);
            NotificationsService = new Mock<INotificationsService>();
            Handler = new RoleAddedNotificationHandler(NotificationsService.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Notification, CancellationToken.None);
        }
    }
}
