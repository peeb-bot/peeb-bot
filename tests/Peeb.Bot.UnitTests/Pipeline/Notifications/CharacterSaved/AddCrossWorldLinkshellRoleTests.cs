using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.XivApi;
using Peeb.Bot.Clients.XivApi.Responses;
using Peeb.Bot.Data;
using Peeb.Bot.Models;
using Peeb.Bot.Pipeline.Notifications.CharacterSaved;
using Peeb.Bot.Pipeline.Notifications.RoleAdded;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Pipeline.Notifications.CharacterSaved
{
    [TestFixture]
    [Parallelizable]
    public class AddCrossWorldLinkshellRoleTests : TestBase<AddCrossWorldLinkshellRoleTestsContext>
    {
        [Test]
        public Task Handle_SettingExistsAndIsEnabledAndCharacterIsCrossWorldLinkshellMember_ShouldAddRole()
        {
            return TestAsync(
                c => c.SetSetting(true).SetIsCrossWorldLinkshellMember(),
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(c.Setting.RoleId, null), Times.Once));
        }

        [Test]
        public Task Handle_SettingExistsAndIsEnabledAndCharacterIsCrossWorldLinkshellMember_ShouldPublishNotification()
        {
            return TestAsync(
                c => c.SetSetting(true).SetIsCrossWorldLinkshellMember(),
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

        [Test]
        public Task Handle_SettingExistsAndIsEnabledAndCrossWorldLinkshellDoesNotExist_ShouldDisableSetting()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c =>
                {
                    c.Setting.IsEnabled.Should().BeFalse();
                    c.Setting.Updated.Should().Be(c.UtcNow);
                });
        }

        [Test]
        public Task Handle_SettingExistsAndIsEnabledAndCrossWorldLinkshellDoesNotExist_ShouldNotAddRole()
        {
            return TestAsync(
                c => c.SetSetting(true),
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(c.Setting.RoleId, It.IsAny<RequestOptions>()), Times.Never));
        }

        [Test]
        public Task Handle_SettingExistsAndIsEnabledAndCharacterIsNotCrossWorldLinkshellMember_ShouldNotAddRole()
        {
            return TestAsync(
                c => c.SetSetting(true).SetIsNotCrossWorldLinkshellMember(),
                c => c.Handle(),
                c => c.User.Verify(u => u.AddRoleAsync(c.Setting.RoleId, It.IsAny<RequestOptions>()), Times.Never));
        }
    }

    public class AddCrossWorldLinkshellRoleTestsContext
    {
        public Mock<IDateTimeOffsetService> DateTimeOffsetService { get; set; }
        public PeebDbContext Db { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public Mock<IGuild> Guild { get; set; }
        public AddCrossWorldLinkshellRoleHandler Handler { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public CharacterSavedNotification Notification { get; set; }
        public AddCrossWorldLinkshellRoleSetting Setting { get; set; }
        public Mock<IGuildUser> User { get; set; }
        public DateTimeOffset UtcNow { get; set; }
        public Mock<IXivApiClient> XivApiClient { get; set; }

        public AddCrossWorldLinkshellRoleTestsContext()
        {
            DateTimeOffsetService = new Mock<IDateTimeOffsetService>();
            Db = new FakePeebDbContext();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Guild = new Mock<IGuild>();
            Mediator = new Mock<IMediator>();
            Notification = new CharacterSavedNotification(Guid.NewGuid(), 1, 2, 3, "Earth", "Foo", "Bar", "https://foo.bar", DateTimeOffset.UtcNow, null);
            User = new Mock<IGuildUser>();
            UtcNow = DateTimeOffset.UtcNow;
            XivApiClient = new Mock<IXivApiClient>();

            DateTimeOffsetService.SetupGet(s => s.UtcNow).Returns(UtcNow);
            DiscordSocketClient.Setup(c => c.GetGuildAsync(Notification.GuildId, CacheMode.AllowDownload, null)).ReturnsAsync(Guild.Object);
            Guild.Setup(g => g.GetUserAsync(Notification.UserId, CacheMode.AllowDownload, null)).ReturnsAsync(User.Object);

            Handler = new AddCrossWorldLinkshellRoleHandler(DateTimeOffsetService.Object, DiscordSocketClient.Object, Mediator.Object, XivApiClient.Object, Db);
        }

        public Task Handle()
        {
            return Handler.Handle(Notification, CancellationToken.None);
        }

        public AddCrossWorldLinkshellRoleTestsContext SetSetting(bool isEnabled)
        {
            Setting = new AddCrossWorldLinkshellRoleSetting(Notification.GuildId, "4", 5, isEnabled, DateTimeOffset.UtcNow);

            Db.AddCrossWorldLinkshellRoleSettings.Add(Setting);
            Db.SaveChanges();

            return this;
        }

        public AddCrossWorldLinkshellRoleTestsContext SetIsCrossWorldLinkshellMember()
        {
            XivApiClient
                .Setup(c => c.GetCrossWorldLinkshell(Setting.CrossWorldLinkshellId))
                .ReturnsAsync(new CrossWorldLinkshellResponse
                {
                    Linkshell = new CrossworldLinkshellDetailResponse
                    {
                        Results = new List<CharacterSummaryResponse>
                        {
                            new()
                            {
                                Id = Notification.LodestoneId
                            }
                        }
                    }
                });

            return this;
        }

        public AddCrossWorldLinkshellRoleTestsContext SetIsNotCrossWorldLinkshellMember()
        {
            XivApiClient
                .Setup(c => c.GetCrossWorldLinkshell(Setting.CrossWorldLinkshellId))
                .ReturnsAsync(new CrossWorldLinkshellResponse
                {
                    Linkshell = new CrossworldLinkshellDetailResponse
                    {
                        Results = new List<CharacterSummaryResponse>()
                    }
                });

            return this;
        }
    }
}
