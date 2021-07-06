using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Helpers;
using Peeb.Bot.Timers;

namespace Peeb.Bot.UnitTests.Clients.Discord.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class WorkingNotifierHandlerTests : TestBase<WorkingNotifierHandlerTestsContext>
    {
        [Test]
        public Task CommandExecuting_ShouldCreateTimer()
        {
            return TestAsync(
                c => c.CommandExecuting(),
                c => c.TimerFactory.Verify(f => f.CreateTimer(It.IsAny<Action>(), TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan), Times.Once));
        }

        [Test]
        public Task CommandExecuting_TimerElapsed_ShouldAddReaction()
        {
            return TestAsync(
                c => c.SetTimerElapsed(),
                c => c.CommandExecuting(),
                c => c.Message.Verify(m => m.AddReactionAsync(It.Is<Emoji>(e => e.Name == EmojiHelper.Clock2.Name), null), Times.Once));
        }

        [Test]
        public Task CommandExecuting_TimerNotElapsed_ShouldNotAddReaction()
        {
            return TestAsync(
                c => c.CommandExecuting(),
                c => c.Message.Verify(m => m.AddReactionAsync(It.IsAny<Emoji>(), It.IsAny<RequestOptions>()), Times.Never));
        }

        [Test]
        public Task ResultExecuting_ShouldDisposeTimer()
        {
            return TestAsync(
                c => c.ResultExecuting(),
                c => c.Timer.Verify(t => t.DisposeAsync(), Times.Once));
        }

        [Test]
        public Task ResultExecuting_TimerElapsed_ShouldRemoveReaction()
        {
            return TestAsync(
                c => c.SetTimerElapsed(),
                c => c.ResultExecuting(),
                c => c.Message.Verify(m => m.RemoveReactionAsync(It.Is<Emoji>(e => e.Name == EmojiHelper.Clock2.Name), c.CurrentUser.Object, null), Times.Once));
        }

        [Test]
        public Task ResultExecuting_TimerNotElapsed_ShouldNotRemoveReaction()
        {
            return TestAsync(
                c => c.ResultExecuting(),
                c => c.Message.Verify(m => m.RemoveReactionAsync(It.IsAny<Emoji>(), It.IsAny<IUser>(), It.IsAny<RequestOptions>()), Times.Never));
        }
    }

    public class WorkingNotifierHandlerTestsContext
    {
        public Mock<ICommandContext> CommandContext { get; set; }
        public Mock<ISelfUser> CurrentUser { get; set; }
        public ICommandHandler Handler { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public MockSequence Sequence { get; set; }
        public Mock<ITimer> Timer { get; set; }
        public Mock<ITimerFactory> TimerFactory { get; set; }

        public WorkingNotifierHandlerTestsContext()
        {
            CommandContext = new Mock<ICommandContext>();
            CurrentUser = new Mock<ISelfUser>();
            Message = new Mock<IUserMessage>(MockBehavior.Strict);
            Sequence = new MockSequence();
            Timer = new Mock<ITimer>(MockBehavior.Strict);
            TimerFactory = new Mock<ITimerFactory>();

            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            CommandContext.SetupGet(c => c.Client.CurrentUser).Returns(CurrentUser.Object);
            Message.Setup(m => m.AddReactionAsync(It.IsAny<Emoji>(), It.IsAny<RequestOptions>())).Returns(Task.CompletedTask);
            Timer.SetupGet(t => t.Elapsed).Returns(false);
            TimerFactory.Setup(f => f.CreateTimer(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(Timer.Object);

            Timer
                .InSequence(Sequence)
                .Setup(t => t.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            Message
                .InSequence(Sequence)
                .Setup(m => m.RemoveReactionAsync(It.IsAny<Emoji>(), It.IsAny<IUser>(), It.IsAny<RequestOptions>()))
                .Returns(Task.CompletedTask);

            Handler = new WorkingNotifierHandler(TimerFactory.Object);
        }

        public WorkingNotifierHandlerTestsContext SetTimerElapsed()
        {
            Timer.SetupGet(t => t.Elapsed).Returns(true);

            TimerFactory
                .Setup(f => f.CreateTimer(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Action callback, TimeSpan _, TimeSpan _) => callback())
                .Returns(Timer.Object);

            return this;
        }

        public Task CommandExecuting()
        {
            return Handler.CommandExecuting(CommandContext.Object);
        }

        public async Task ResultExecuting()
        {
            await CommandExecuting();
            await Handler.ResultExecuting(null, CommandContext.Object, null);
        }
    }
}
