using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord.Handlers;

namespace Peeb.Bot.UnitTests.Clients.Discord.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class TypingNotifierHandlerTests : TestBase<TypingNotifierHandlerTestsContext>
    {
        [Test]
        public Task CommandExecuting_ShouldEnterTypingState()
        {
            return TestAsync(
                c => c.CommandExecuting(),
                c => c.Channel.Verify(ch => ch.EnterTypingState(null), Times.Once));
        }

        [Test]
        public Task ResultExecuting_ShouldDisposeTypingNotifier()
        {
            return TestAsync(
                c => c.ResultExecuting(),
                c => c.TypingNotifier.Verify(n => n.Dispose(), Times.Once));
        }
    }

    public class TypingNotifierHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public Mock<ICommandContext> CommandContext { get; set; }
        public ICommandHandler Handler { get; set; }
        public Mock<IDisposable> TypingNotifier { get; set; }

        public TypingNotifierHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>();
            CommandContext = new Mock<ICommandContext>();
            TypingNotifier = new Mock<IDisposable>();

            Channel.Setup(c => c.EnterTypingState(null)).Returns(TypingNotifier.Object);
            CommandContext.SetupGet(c => c.Channel).Returns(Channel.Object);

            Handler = new TypingNotifierHandler();
        }

        public Task CommandExecuting()
        {
            return Handler.CommandExecuting(CommandContext.Object);
        }

        public async Task ResultExecuting()
        {
            await CommandExecuting();
            await Handler.ResultExecuting(null, null, null);
        }

    }
}
