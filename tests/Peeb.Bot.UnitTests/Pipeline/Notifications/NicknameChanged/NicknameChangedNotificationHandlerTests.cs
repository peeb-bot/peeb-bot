using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Pipeline.Notifications.NicknameChanged;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Pipeline.Notifications.NicknameChanged
{
    [TestFixture]
    [Parallelizable]
    public class NicknameChangedNotificationHandlerTests : TestBase<NicknameChangedNotificationHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldAddNotification()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.NotificationsService.Verify(s => s.AddNotification(c.Notification), Times.Once));
        }
    }

    public class NicknameChangedNotificationHandlerTestsContext
    {
        public NicknameChangedNotificationHandler Handler { get; set; }
        public NicknameChangedNotification Notification { get; set; }
        public Mock<INotificationsService> NotificationsService { get; set; }

        public NicknameChangedNotificationHandlerTestsContext()
        {
            Notification = new NicknameChangedNotification(1, "Foo Bar");
            NotificationsService = new Mock<INotificationsService>();
            Handler = new NicknameChangedNotificationHandler(NotificationsService.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Notification, CancellationToken.None);
        }
    }
}
