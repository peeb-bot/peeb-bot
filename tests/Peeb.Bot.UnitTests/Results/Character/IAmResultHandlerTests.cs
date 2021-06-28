using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Dtos;
using Peeb.Bot.Helpers;
using Peeb.Bot.Notifications.NicknameChanged;
using Peeb.Bot.Notifications.RoleAdded;
using Peeb.Bot.Results.Character;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Results.Character
{
    [TestFixture]
    [Parallelizable]
    public class IAmResultHandlerTests : TestBase<IAmResultHandlerTestsContext>
    {
        [Test]
        public Task Handle_NicknameChangedNotificationDoesNotExistAndRoleAddedNotificationsDoNotExist_ShouldSendMessage()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Success &&
                            e.Title == "Foo Bar (Earth)" &&
                            e.Thumbnail.Value.Url == "https://foo.bar" &&
                            e.Description == "Character saved." &&
                            e.Fields.IsEmpty),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }

        [Test]
        public Task Handle_NicknameChangedNotificationExistsAndRoleAddedNotificationsDoNotExist_ShouldSendMessage()
        {
            return TestAsync(
                c => c.SetNicknameChangedNotification(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Success &&
                            e.Title == "Foo Bar (Earth)" &&
                            e.Thumbnail.Value.Url == "https://foo.bar" &&
                            e.Description == "Character saved." &&
                            e.Fields.Length == 1 &&
                            e.Fields[0].Name == "Nickname" &&
                            e.Fields[0].Value == "Your Discord nickname was changed to **Foo Bar**"),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }

        [Test]
        public Task Handle_NicknameChangedNotificationDoesNotExistAndRoleAddedNotificationsExist_ShouldSendMessage()
        {
            return TestAsync(
                c => c.SetRoleAddedNotifications(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Success &&
                            e.Title == "Foo Bar (Earth)" &&
                            e.Thumbnail.Value.Url == "https://foo.bar" &&
                            e.Description == "Character saved." &&
                            e.Fields.Length == 1 &&
                            e.Fields[0].Name == "Roles Added" &&
                            e.Fields[0].Value == "<@&3> <@&4>"),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }

        [Test]
        public Task Handle_NicknameChangedNotificationExistsAndRoleAddedNotificationsExist_ShouldSendMessage()
        {
            return TestAsync(
                c => c.SetNicknameChangedNotification().SetRoleAddedNotifications(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Success &&
                            e.Title == "Foo Bar (Earth)" &&
                            e.Thumbnail.Value.Url == "https://foo.bar" &&
                            e.Description == "Character saved." &&
                            e.Fields.Length == 2 &&
                            e.Fields[0].Name == "Nickname" &&
                            e.Fields[0].Value == "Your Discord nickname was changed to **Foo Bar**" &&
                            e.Fields[1].Name == "Roles Added" &&
                            e.Fields[1].Value == "<@&3> <@&4>"),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }
    }

    public class IAmResultHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public CharacterDto Character { get; set; }
        public Mock<ICommandContext> CommandContext { get; set; }
        public IAmResultHandler Handler { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public Mock<INotificationsService> NotificationsService { get; set; }
        public IAmResult Result { get; set; }

        public IAmResultHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>();
            Character = new CharacterDto("Earth", "Foo", "Bar", "https://foo.bar");
            CommandContext = new Mock<ICommandContext>();
            Message = new Mock<IUserMessage>();
            NotificationsService = new Mock<INotificationsService>();
            Result = new IAmResult(Character);

            CommandContext.SetupGet(c => c.Channel).Returns(Channel.Object);
            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            Message.SetupGet(m => m.Id).Returns(1);
            NotificationsService.Setup(s => s.GetNotifications<RoleAddedNotification>()).Returns(new List<RoleAddedNotification>());

            Handler = new IAmResultHandler(NotificationsService.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(new Optional<CommandInfo>(), CommandContext.Object, Result);
        }

        public IAmResultHandlerTestsContext SetNicknameChangedNotification()
        {
            NotificationsService
                .Setup(s => s.GetNotification<NicknameChangedNotification>())
                .Returns(new NicknameChangedNotification(2, "Foo Bar"));

            return this;
        }

        public IAmResultHandlerTestsContext SetRoleAddedNotifications()
        {
            NotificationsService
                .Setup(s => s.GetNotifications<RoleAddedNotification>())
                .Returns(new List<RoleAddedNotification>
                {
                    new(2, 3),
                    new(2, 4)
                });

            return this;
        }
    }
}
