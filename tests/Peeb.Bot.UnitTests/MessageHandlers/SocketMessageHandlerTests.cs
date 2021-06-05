using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;
using Peeb.Bot.MessageHandlers;
using Peeb.Bot.Settings;

namespace Peeb.Bot.UnitTests.MessageHandlers
{
    [TestFixture]
    [Parallelizable]
    public class SocketMessageHandlerTests : TestBase<SocketMessageHandlerContext>
    {
        [Test]
        public Task MessageReceived_ShouldExecuteCommand()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?"),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(cs => cs.ExecuteAsync(
                        It.Is<CommandContext>(cc =>
                            cc.Client == c.DiscordSocketClient.Object &&
                            cc.Message == c.Message),
                        1,
                        c.ServiceProvider.Object,
                        MultiMatchHandling.Exception),
                    Times.Once));
        }

        [Test]
        public Task MessageReceived_MessageIsNotUserMessage_ShouldNotExecuteCommand()
        {
            return TestAsync(
                c => c.SetSystemMessage(),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(c => c.ExecuteAsync(
                    It.IsAny<CommandContext>(),
                    It.IsAny<int>(),
                    It.IsAny<IServiceProvider>(),
                    It.IsAny<MultiMatchHandling>()),
                    Times.Never));
        }

        [Test]
        public Task MessageReceived_MessageSourceIsNotUser_ShouldNotExecuteCommand()
        {
            return TestAsync(
                c => c.SetUserMessage().SetSystemMessageSource(),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(c => c.ExecuteAsync(
                        It.IsAny<CommandContext>(),
                        It.IsAny<int>(),
                        It.IsAny<IServiceProvider>(),
                        It.IsAny<MultiMatchHandling>()),
                    Times.Never));
        }

        [Test]
        public Task MessageReceived_MessagePrefixIsNotAMatch_ShouldNotExecuteCommand()
        {
            return TestAsync(
                c => c.SetUserMessage().SetMessagePrefix("!"),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(c => c.ExecuteAsync(
                        It.IsAny<CommandContext>(),
                        It.IsAny<int>(),
                        It.IsAny<IServiceProvider>(),
                        It.IsAny<MultiMatchHandling>()),
                    Times.Never));
        }

        [Test]
        public Task MessageReceived_CommandErrors_ShouldReplyWithErrorReason()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetUnknownCommandError(),
                c => c.MessageReceived(),
                c => c.Channel.Verify(ch => ch.SendMessageAsync(
                        c.Result.Object.ErrorReason,
                        false,
                        null,
                        null,
                        null,
                        It.Is<MessageReference>(m => m.MessageId.Value == c.Message.Id)),
                    Times.Once));
        }
    }

    public class SocketMessageHandlerContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public Mock<ICommandService> CommandService { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public IMessage Message { get; set; }
        public Mock<IOptionsMonitor<DiscordSettings>> OptionsMonitor { get; set; }
        public Mock<IResult> Result { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public DiscordSettings Settings { get; set; }
        public SocketMessageHandler SocketMessageHandler { get; set; }

        public SocketMessageHandlerContext()
        {
            Channel = new Mock<IMessageChannel>();
            CommandService = new Mock<ICommandService>();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            OptionsMonitor = new Mock<IOptionsMonitor<DiscordSettings>>();
            Result = new Mock<IResult>();
            ServiceProvider = new Mock<IServiceProvider>();
            Settings = new DiscordSettings { Prefix = "?" };

            CommandService.Setup(c => c.ExecuteAsync(
                    It.IsAny<CommandContext>(),
                    It.IsAny<int>(),
                    It.IsAny<IServiceProvider>(),
                    It.IsAny<MultiMatchHandling>()))
                .ReturnsAsync(Result.Object);

            OptionsMonitor.Setup(s => s.CurrentValue).Returns(Settings);

            SocketMessageHandler = new SocketMessageHandler(CommandService.Object, DiscordSocketClient.Object, OptionsMonitor.Object, ServiceProvider.Object);
        }

        public SocketMessageHandlerContext SetUnknownCommandError()
        {
            Result.SetupGet(r => r.Error).Returns(CommandError.UnknownCommand);
            Result.SetupGet(r => r.ErrorReason).Returns("Unknown command.");

            return this;
        }

        public SocketMessageHandlerContext SetMessagePrefix(string prefix)
        {
            Mock.Get(Message).SetupGet(m => m.Content).Returns($"{prefix}ping");

            return this;
        }

        public SocketMessageHandlerContext SetSystemMessage()
        {
            Message = Mock.Of<ISystemMessage>();

            return this;
        }

        public SocketMessageHandlerContext SetSystemMessageSource()
        {
            Mock.Get(Message).SetupGet(m => m.Source).Returns(MessageSource.System);

            return this;
        }

        public SocketMessageHandlerContext SetUserMessage()
        {
            Message = Mock.Of<IUserMessage>(m => m.Id == 1 && m.Channel == Channel.Object);

            return this;
        }

        public SocketMessageHandlerContext SetUserMessageSource()
        {
            Mock.Get(Message).SetupGet(m => m.Source).Returns(MessageSource.User);

            return this;
        }

        public Task MessageReceived()
        {
            return SocketMessageHandler.MessageReceived(Message);
        }
    }
}
