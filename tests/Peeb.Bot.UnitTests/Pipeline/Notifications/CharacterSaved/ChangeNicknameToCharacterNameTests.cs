using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Data;
using Peeb.Bot.Models;
using Peeb.Bot.Pipeline.Notifications.CharacterSaved;
using Peeb.Bot.Pipeline.Notifications.NicknameChanged;

namespace Peeb.Bot.UnitTests.Pipeline.Notifications.CharacterSaved
{
    [TestFixture]
    [Parallelizable]
    public class ChangeNicknameToCharacterNameTests : TestBase<ChangeNicknameToCharacterNameTestsContext>
    {
        [Test]
        public Task Handle_SettingExistsAndIsEnabled_ShouldChangeNickname()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c =>
                {
                    c.GuildUserProperties.Nickname.IsSpecified.Should().BeTrue();
                    c.GuildUserProperties.Nickname.Value.Should().Be(c.Notification.Name);
                });
        }

        [Test]
        public Task Handle_SettingExistsAndIsEnabled_ShouldPublishNotification()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c => c.Mediator.Verify(
                    m => m.Publish(It.Is<NicknameChangedNotification>(n => n.UserId == c.Notification.UserId && n.Nickname == c.Notification.Name), CancellationToken.None),
                    Times.Once));
        }

        [Test]
        public Task Handle_SettingDoesNotExist_ShouldNotChangeNickname()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.GuildUserProperties.Nickname.IsSpecified.Should().BeFalse());
        }

        [Test]
        public Task Handle_SettingExistsAndIsNotEnabled_ShouldNotChangeNickname()
        {
            return TestAsync(
                c => c.SetSetting(false),
                c => c.Handle(),
                c =>
                {
                    c.User.Verify(u => u.ModifyAsync(It.IsAny<Action<GuildUserProperties>>(), It.IsAny<RequestOptions>()), Times.Never);
                    c.GuildUserProperties.Nickname.IsSpecified.Should().BeFalse();
                });
        }
    }

    public class ChangeNicknameToCharacterNameTestsContext
    {
        public PeebDbContext Db { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public Mock<IGuild> Guild { get; set; }
        public GuildUserProperties GuildUserProperties { get; set; }
        public ChangeNicknameToCharacterNameHandler Handler { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CharacterSavedNotification Notification { get; set; }
        public ChangeNicknameToCharacterNameSetting Setting { get; set; }
        public Mock<IGuildUser> User { get; set; }

        public ChangeNicknameToCharacterNameTestsContext()
        {
            Db = new FakePeebDbContext();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Guild = new Mock<IGuild>();
            GuildUserProperties = new GuildUserProperties();
            Mediator = new Mock<IMediator>();
            Notification = new CharacterSavedNotification(Guid.NewGuid(), 1, 2, 3, "Earth", "Foo", "Bar", "https://foo.bar", DateTimeOffset.UtcNow, null);
            User = new Mock<IGuildUser>();

            DiscordSocketClient.Setup(c => c.GetGuildAsync(Notification.GuildId, CacheMode.AllowDownload, null)).ReturnsAsync(Guild.Object);
            Guild.Setup(g => g.GetUserAsync(Notification.UserId, CacheMode.AllowDownload, null)).ReturnsAsync(User.Object);

            User
                .Setup(u => u.ModifyAsync(It.IsAny<Action<GuildUserProperties>>(), It.IsAny<RequestOptions>()))
                .Callback((Action<GuildUserProperties> func, RequestOptions _) => func(GuildUserProperties));

            Handler = new ChangeNicknameToCharacterNameHandler(DiscordSocketClient.Object, Mediator.Object, Db);
        }

        public Task Handle()
        {
            return Handler.Handle(Notification, CancellationToken.None);
        }

        public ChangeNicknameToCharacterNameTestsContext SetSetting(bool isEnabled)
        {
            Setting = new ChangeNicknameToCharacterNameSetting(Notification.GuildId, isEnabled, DateTimeOffset.UtcNow);

            Db.ChangeNicknameToCharacterNameSettings.Add(Setting);
            Db.SaveChanges();

            return this;
        }
    }
}
