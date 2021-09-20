using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Handlers;
using Peeb.Bot.Helpers;
using Peeb.Bot.Timers;

namespace Peeb.Bot.UnitTests.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class WorkingNotifierHandlerTests : TestBase<WorkingNotifierHandlerTestsContext>
    {
        [Test]
        public Task CommandExecuting_ShouldStartTimer()
        {
            return TestAsync(
                c => c.CommandExecuting(),
                c => c.Timer.Verify(t => t.Start(It.IsAny<Func<Task>>(), TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan), Times.Once));
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
        public Task ResultExecuting_ShouldStopTimer()
        {
            return TestAsync(
                c => c.ResultExecuting(),
                c => c.Timer.Verify(t => t.Stop(), Times.Once));
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
        public Mock<IAsyncTimer> Timer { get; set; }

        public WorkingNotifierHandlerTestsContext()
        {
            CommandContext = new Mock<ICommandContext>();
            CurrentUser = new Mock<ISelfUser>();
            Message = new Mock<IUserMessage>(MockBehavior.Strict);
            Sequence = new MockSequence();
            Timer = new Mock<IAsyncTimer>(MockBehavior.Strict);

            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            CommandContext.SetupGet(c => c.Client.CurrentUser).Returns(CurrentUser.Object);
            Message.Setup(m => m.AddReactionAsync(It.IsAny<Emoji>(), It.IsAny<RequestOptions>())).Returns(Task.CompletedTask);
            Timer.Setup(t => t.Start(It.IsAny<Func<Task>>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()));
            Timer.SetupGet(t => t.Elapsed).Returns(false);

            Timer
                .InSequence(Sequence)
                .Setup(t => t.Stop())
                .Returns(Task.CompletedTask);

            Message
                .InSequence(Sequence)
                .Setup(m => m.RemoveReactionAsync(It.IsAny<Emoji>(), It.IsAny<IUser>(), It.IsAny<RequestOptions>()))
                .Returns(Task.CompletedTask);

            Handler = new WorkingNotifierHandler(Timer.Object);
        }

        public WorkingNotifierHandlerTestsContext SetTimerElapsed()
        {
            Timer
                .Setup(t => t.Start(It.IsAny<Func<Task>>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Func<Task> callback, TimeSpan _, TimeSpan _) => callback());

            Timer.SetupGet(t => t.Elapsed).Returns(true);

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
