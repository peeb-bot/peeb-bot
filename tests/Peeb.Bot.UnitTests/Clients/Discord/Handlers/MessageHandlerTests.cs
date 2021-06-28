using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.Discord.Caches;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.Results.Ok;
using Peeb.Bot.Settings;

namespace Peeb.Bot.UnitTests.Clients.Discord.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class MessageHandlerTests : TestBase<MessageHandlerTestsContext>
    {
        [Test]
        public Task MessageReceived_ShouldCreateServiceScope()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.MessageReceived(),
                c => c.ServiceScopeFactory.Verify(f => f.CreateScope(), Times.Once));
        }

        [Test]
        public Task MessageReceived_ShouldCallCommandHandlersCommandExecutingMethod()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.MessageReceived(),
                c => c.CommandHandlers.ForEach(h => h.Verify(
                    ch => ch.CommandExecuting(
                        It.Is<CommandContext>(cc =>
                            cc.Client == c.DiscordSocketClient.Object &&
                            cc.Message == c.Message)),
                    Times.Once)));
        }

        [Test]
        public Task MessageReceived_ShouldExecuteCommand()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(
                    cs => cs.ExecuteAsync(
                        It.Is<CommandContext>(cc =>
                            cc.Client == c.DiscordSocketClient.Object &&
                            cc.Message == c.Message),
                        1,
                        c.ServiceScopeServiceProvider.Object,
                        MultiMatchHandling.Exception),
                    Times.Once));
        }

        [Test]
        public Task MessageReceived_MessageIsNotUserMessage_ShouldNotExecuteCommand()
        {
            return TestAsync(
                c => c.SetSystemMessage(),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(
                    c => c.ExecuteAsync(
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
                c => c.CommandService.Verify(
                    cs => cs.ExecuteAsync(
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
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("!"),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(
                    cs => cs.ExecuteAsync(
                        It.IsAny<CommandContext>(),
                        It.IsAny<int>(),
                        It.IsAny<IServiceProvider>(),
                        It.IsAny<MultiMatchHandling>()),
                    Times.Never));
        }

        [Test]
        public Task MessageReceived_UnknownCommand_ShouldNotExecuteCommand()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetUnknownCommand(),
                c => c.MessageReceived(),
                c => c.CommandService.Verify(
                    cs => cs.ExecuteAsync(
                        It.IsAny<CommandContext>(),
                        It.IsAny<int>(),
                        It.IsAny<IServiceProvider>(),
                        It.IsAny<MultiMatchHandling>()),
                    Times.Never));
        }

        [Test]
        public Task CommandExecuted_ShouldCallCommandHandlersResultExecutingMethod()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.CommandExecuted(),
                c => c.CommandHandlers.ForEach(h => h.Verify(
                    ch => ch.ResultExecuting(
                        It.IsAny<Optional<CommandInfo>>(),
                        c.CommandContext,
                        c.Result),
                    Times.Once)));
        }

        [Test]
        public Task CommandExecuted_ResultHandlerExists_ShouldHandleResult()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand().SetResultHandler(),
                c => c.CommandExecuted(),
                c => c.ResultHandler.Verify(h => h.Handle(It.IsAny<Optional<CommandInfo>>(), c.CommandContext, c.Result), Times.Once));
        }

        [Test]
        public Task CommandExecuted_ShouldCallCommandHandlersCommandExecutedMethod()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.CommandExecuted(),
                c => c.CommandHandlers.ForEach(h => h.Verify(
                    ch => ch.CommandExecuted(
                        It.IsAny<Optional<CommandInfo>>(),
                        c.CommandContext,
                        c.Result),
                    Times.Once)));
        }

        [Test]
        public Task CommandExecuted_ShouldDisposeServiceScope()
        {
            return TestAsync(
                c => c.SetUserMessage().SetUserMessageSource().SetMessagePrefix("?").SetKnownCommand(),
                c => c.CommandExecuted(),
                c => c.ServiceScope.Verify(s => s.Dispose(), Times.Once));
        }
    }

    public class MessageHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public ICommandContext CommandContext { get; set; }
        public MockSequence CommandExecutedSequence { get; set; }
        public Mock<ICommandHandler> CommandHandler1 { get; set; }
        public Mock<ICommandHandler> CommandHandler2 { get; set; }
        public List<Mock<ICommandHandler>> CommandHandlers { get; set; }
        public Mock<ICommandService> CommandService { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public IMessage Message { get; set; }
        public MessageHandler MessageHandler { get; set; }
        public MockSequence MessageReceivedSequence { get; set; }
        public Mock<IOptionsMonitor<DiscordSettings>> OptionsMonitor { get; set; }
        public OkResult Result { get; set; }
        public Mock<IResultHandler> ResultHandler { get; set; }
        public SearchResult SearchResult { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public Mock<IServiceScope> ServiceScope { get; set; }
        public Mock<IServiceScopeCache> ServiceScopeCache { get; set; }
        public Mock<IServiceScopeFactory> ServiceScopeFactory { get; set; }
        public Mock<IServiceProvider> ServiceScopeServiceProvider { get; set; }
        public DiscordSettings Settings { get; set; }

        public MessageHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>(MockBehavior.Strict);
            CommandExecutedSequence = new MockSequence();
            CommandHandler1 = new Mock<ICommandHandler>(MockBehavior.Strict);
            CommandHandler2 = new Mock<ICommandHandler>(MockBehavior.Strict);
            CommandHandlers = new List<Mock<ICommandHandler>> { CommandHandler1, CommandHandler2 };
            CommandService = new Mock<ICommandService>(MockBehavior.Strict);
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            MessageReceivedSequence = new MockSequence();
            OptionsMonitor = new Mock<IOptionsMonitor<DiscordSettings>>();
            Result = new OkResult("OK");
            ServiceProvider = new Mock<IServiceProvider>();
            ServiceScope = new Mock<IServiceScope>(MockBehavior.Strict);
            ServiceScopeCache = new Mock<IServiceScopeCache>();
            ServiceScopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
            ServiceScopeServiceProvider = new Mock<IServiceProvider>();
            Settings = new DiscordSettings { Prefix = "?" };

            OptionsMonitor.SetupGet(s => s.CurrentValue).Returns(Settings);
            ServiceProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(ServiceScopeFactory.Object);
            ServiceScope.SetupGet(s => s.ServiceProvider).Returns(ServiceScopeServiceProvider.Object);
            ServiceScopeCache.Setup(c => c.Get()).Returns(ServiceScope.Object);
            ServiceScopeServiceProvider.Setup(p => p.GetService(typeof(IEnumerable<ICommandHandler>))).Returns(CommandHandlers.Select(h => h.Object));

            MessageHandler = new MessageHandler(
                CommandService.Object,
                DiscordSocketClient.Object,
                OptionsMonitor.Object,
                ServiceProvider.Object,
                ServiceScopeCache.Object);
        }

        public MessageHandlerTestsContext SetMessagePrefix(string prefix)
        {
            Mock.Get(Message).SetupGet(m => m.Content).Returns($"{prefix}ping");

            return this;
        }

        public MessageHandlerTestsContext SetResultHandler()
        {
            ResultHandler = new Mock<IResultHandler>(MockBehavior.Strict);

            ServiceScopeServiceProvider
                .Setup(p => p.GetService(typeof(IResultHandler<OkResult>)))
                .Returns(ResultHandler.Object);

            return this;
        }

        public MessageHandlerTestsContext SetSystemMessage()
        {
            Message = Mock.Of<ISystemMessage>();

            return this;
        }

        public MessageHandlerTestsContext SetSystemMessageSource()
        {
            Mock.Get(Message).SetupGet(m => m.Source).Returns(MessageSource.System);

            return this;
        }

        public MessageHandlerTestsContext SetUserMessage()
        {
            Message = Mock.Of<IUserMessage>(m => m.Id == 1 && m.Channel == Channel.Object);
            CommandContext = new CommandContext(DiscordSocketClient.Object, (IUserMessage)Message);

            return this;
        }

        public MessageHandlerTestsContext SetUserMessageSource()
        {
            Mock.Get(Message).SetupGet(m => m.Source).Returns(MessageSource.User);

            return this;
        }

        public MessageHandlerTestsContext SetKnownCommand()
        {
            SearchResult = SearchResult.FromSuccess("", new List<CommandMatch>());

            return this;
        }

        public MessageHandlerTestsContext SetUnknownCommand()
        {
            SearchResult = SearchResult.FromError(CommandError.UnknownCommand, "");

            return this;
        }

        public Task MessageReceived()
        {
            CommandService
                .InSequence(MessageReceivedSequence)
                .Setup(s => s.Search(It.IsAny<ICommandContext>(), It.IsAny<int>()))
                .Returns(SearchResult);

            ServiceScopeFactory
                .InSequence(MessageReceivedSequence)
                .Setup(f => f.CreateScope())
                .Returns(ServiceScope.Object);

            CommandHandler1
                .InSequence(MessageReceivedSequence)
                .Setup(h => h.CommandExecuting(It.IsAny<ICommandContext>()))
                .Returns(Task.CompletedTask);

            CommandHandler2
                .InSequence(MessageReceivedSequence)
                .Setup(h => h.CommandExecuting(It.IsAny<ICommandContext>()))
                .Returns(Task.CompletedTask);

            CommandService
                .InSequence(MessageReceivedSequence)
                .Setup(s => s.ExecuteAsync(It.IsAny<ICommandContext>(), It.IsAny<int>(), It.IsAny<IServiceProvider>(), It.IsAny<MultiMatchHandling>()))
                .ReturnsAsync(Result);

            return MessageHandler.MessageReceived(Message);
        }

        public Task CommandExecuted()
        {
            CommandHandler2
                .InSequence(CommandExecutedSequence)
                .Setup(h => h.ResultExecuting(It.IsAny<Optional<CommandInfo>>(), It.IsAny<ICommandContext>(), It.IsAny<IResult>()))
                .Returns(Task.CompletedTask);

            CommandHandler1
                .InSequence(CommandExecutedSequence)
                .Setup(h => h.ResultExecuting(It.IsAny<Optional<CommandInfo>>(), It.IsAny<ICommandContext>(), It.IsAny<IResult>()))
                .Returns(Task.CompletedTask);

            ResultHandler?.InSequence(CommandExecutedSequence)
                .Setup(h => h.Handle(It.IsAny<Optional<CommandInfo>>(), It.IsAny<ICommandContext>(), It.IsAny<IResult>()))
                .Returns(Task.CompletedTask);

            CommandHandler2
                .InSequence(CommandExecutedSequence)
                .Setup(h => h.CommandExecuted(It.IsAny<Optional<CommandInfo>>(), It.IsAny<ICommandContext>(), It.IsAny<IResult>()))
                .Returns(Task.CompletedTask);

            CommandHandler1
                .InSequence(CommandExecutedSequence)
                .Setup(h => h.CommandExecuted(It.IsAny<Optional<CommandInfo>>(), It.IsAny<ICommandContext>(), It.IsAny<IResult>()))
                .Returns(Task.CompletedTask);

            ServiceScope.InSequence(CommandExecutedSequence).Setup(s => s.Dispose());

            return MessageHandler.CommandExecuted(new Optional<CommandInfo>(), CommandContext, Result);
        }
    }
}
