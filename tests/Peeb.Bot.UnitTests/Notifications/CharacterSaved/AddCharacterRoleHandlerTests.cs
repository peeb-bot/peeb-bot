using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Data;
using Peeb.Bot.Models;
using Peeb.Bot.Notifications.CharacterSaved;
using Peeb.Bot.Notifications.RoleAdded;

namespace Peeb.Bot.UnitTests.Notifications.CharacterSaved
{
    [TestFixture]
    [Parallelizable]
    public class AddCharacterRoleHandlerTests : TestBase<AddCharacterRoleHandlerTestsContext>
    {
        [Test]
        public Task Handle_SettingExistsAndIsEnabled_ShouldAddRole()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(c.Setting.RoleId, null), Times.Once));
        }

        [Test]
        public Task Handle_SettingExistsAndIsEnabled_ShouldPublishNotification()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c => c.Mediator.Verify(
                    m => m.Publish(It.Is<RoleAddedNotification>(n => n.UserId == c.Notification.UserId && n.RoleId == c.Setting.RoleId), CancellationToken.None),
                    Times.Once));
        }

        [Test]
        public Task Handle_SettingDoesNotExist_ShouldNotAddRole()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(It.IsAny<ulong>(), It.IsAny<RequestOptions>()), Times.Never));
        }

        [Test]
        public Task Handle_SettingExistsAndIsNotEnabled_ShouldNotAddRole()
        {
            return TestAsync(
                c => c.SetSetting(false),
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(It.IsAny<ulong>(), It.IsAny<RequestOptions>()), Times.Never));
        }
    }

    public class AddCharacterRoleHandlerTestsContext
    {
        public PeebDbContext Db { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public Mock<IGuild> Guild { get; set; }
        public AddCharacterRoleHandler Handler { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CharacterSavedNotification Notification { get; set; }
        public AddCharacterRoleSetting Setting { get; set; }
        public Mock<IGuildUser> User { get; set; }

        public AddCharacterRoleHandlerTestsContext()
        {
            Db = new FakePeebDbContext();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Guild = new Mock<IGuild>();
            Mediator = new Mock<IMediator>();
            Notification = new CharacterSavedNotification(Guid.NewGuid(), 1, 2, 3, "Earth", "Foo", "Bar", "https://foo.bar", DateTimeOffset.UtcNow, null);
            User = new Mock<IGuildUser>();

            DiscordSocketClient.Setup(c => c.GetGuildAsync(Notification.GuildId, CacheMode.AllowDownload, null)).ReturnsAsync(Guild.Object);
            Guild.Setup(g => g.GetUserAsync(Notification.UserId, CacheMode.AllowDownload, null)).ReturnsAsync(User.Object);

            Handler = new AddCharacterRoleHandler(DiscordSocketClient.Object, Mediator.Object, Db);
        }

        public Task Handle()
        {
            return Handler.Handle(Notification, CancellationToken.None);
        }

        public AddCharacterRoleHandlerTestsContext SetSetting(bool isEnabled)
        {
            Setting = new AddCharacterRoleSetting(Notification.GuildId, 4, isEnabled, DateTimeOffset.UtcNow);

            Db.AddCharacterRoleSettings.Add(Setting);
            Db.SaveChanges();

            return this;
        }
    }
}
