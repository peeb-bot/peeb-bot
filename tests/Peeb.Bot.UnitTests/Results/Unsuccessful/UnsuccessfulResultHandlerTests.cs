using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Helpers;
using Peeb.Bot.Results.Unsuccessful;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Results.Unsuccessful
{
    [TestFixture]
    [Parallelizable]
    public class UnsuccessfulResultHandlerTests : TestBase<UnsuccessfulResultHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldSendMessage()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Error &&
                            e.Title == ":(" &&
                            e.Description == c.Result.Reason),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }

        [Test]
        public Task Handle_ShouldDeleteOriginalMessage()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.DeleteMessageAsync(c.Message.Object.Id, null), Times.Once));
        }

        [Test]
        public Task Handle_ShouldDeleteErrorMessageAfterDelay()
        {
            return TestAsync(
                c => c.Handle(),
                c =>
                {
                    c.TaskService.Verify(s => s.Delay(10000), Times.Once);
                    c.Channel.Verify(ch => ch.DeleteMessageAsync(c.ErrorMessage.Object, null), Times.Once);
                });
        }
    }

    public class UnsuccessfulResultHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public Mock<ICommandContext> CommandContext { get; set; }
        public Mock<IUserMessage> ErrorMessage { get; set; }
        public UnsuccessfulResultHandler Handler { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public UnsuccessfulResult Result { get; set; }
        public MockSequence Sequence { get; set; }
        public Mock<ITaskService> TaskService { get; set; }

        public UnsuccessfulResultHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>(MockBehavior.Strict);
            CommandContext = new Mock<ICommandContext>();
            ErrorMessage = new Mock<IUserMessage>();
            Message = new Mock<IUserMessage>();
            Result = new UnsuccessfulResult("Foobar.");
            Sequence = new MockSequence();
            TaskService = new Mock<ITaskService>(MockBehavior.Strict);

            Channel
                .InSequence(Sequence)
                .Setup(c => c.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(ErrorMessage.Object);

            Channel
                .InSequence(Sequence)
                .Setup(c => c.DeleteMessageAsync(It.IsAny<ulong>(), It.IsAny<RequestOptions>()))
                .Returns(Task.CompletedTask);

            TaskService
                .InSequence(Sequence)
                .Setup(s => s.Delay(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            Channel
                .InSequence(Sequence)
                .Setup(c => c.DeleteMessageAsync(It.IsAny<IMessage>(), It.IsAny<RequestOptions>()))
                .Returns(Task.CompletedTask);

            CommandContext.SetupGet(c => c.Channel).Returns(Channel.Object);
            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            Message.SetupGet(m => m.Id).Returns(1);

            Handler = new UnsuccessfulResultHandler(TaskService.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(new Optional<CommandInfo>(), CommandContext.Object, Result);
        }
    }
}
