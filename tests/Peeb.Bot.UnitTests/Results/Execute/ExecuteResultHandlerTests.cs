using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Helpers;
using Peeb.Bot.Results.Execute;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Results.Execute
{
    [TestFixture]
    [Parallelizable]
    public class ExecuteResultHandlerTests : TestBase<ExecuteResultHandlerTestsContext>
    {
        [Test]
        public Task Handle_IsNotSuccess_ShouldSendMessage()
        {
            return TestAsync(
                c => c.SetIsNotSuccess(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        null,
                        false,
                        It.Is<Embed>(e =>
                            e.Color == ColorHelper.Error &&
                            e.Title == ":(" &&
                            e.Description == "Sorry, something went wrong. Please check your command and try again."),
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }

        [Test]
        public Task Handle_IsNotSuccess_ShouldDeleteOriginalMessage()
        {
            return TestAsync(
                c => c.SetIsNotSuccess(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.DeleteMessageAsync(c.Message.Object.Id, null), Times.Once));
        }

        [Test]
        public Task Handle_IsNotSuccess_ShouldDeleteErrorMessageAfterDelay()
        {
            return TestAsync(
                c => c.SetIsNotSuccess(),
                c => c.Handle(),
                c =>
                {
                    c.TaskService.Verify(s => s.Delay(10000), Times.Once);
                    c.Channel.Verify(ch => ch.DeleteMessageAsync(c.ErrorMessage.Object, null), Times.Once);
                });
        }

        [Test]
        public Task Handle_IsSuccess_ShouldNotSendMessage()
        {
            return TestAsync(
                c => c.SetIsSuccess(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        It.IsAny<string>(),
                        It.IsAny<bool>(),
                        It.IsAny<Embed>(),
                        It.IsAny<RequestOptions>(),
                        It.IsAny<AllowedMentions>(),
                        It.IsAny<MessageReference>()),
                    Times.Never));
        }

        [Test]
        public Task Handle_IsSuccess_ShouldNotDeleteOriginalMessage()
        {
            return TestAsync(
                c => c.SetIsSuccess(),
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.DeleteMessageAsync(c.Message.Object.Id, null), Times.Never));
        }
    }

    public class ExecuteResultHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public Mock<ICommandContext> CommandContext { get; set; }
        public Mock<IUserMessage> ErrorMessage { get; set; }
        public ExecuteResultHandler Handler { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public ExecuteResult Result { get; set; }
        public MockSequence Sequence { get; set; }
        public Mock<ITaskService> TaskService { get; set; }

        public ExecuteResultHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>(MockBehavior.Strict);
            CommandContext = new Mock<ICommandContext>();
            ErrorMessage = new Mock<IUserMessage>();
            Message = new Mock<IUserMessage>();
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

            CommandContext.SetupGet(c => c.Channel).Returns(Channel.Object);
            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            Message.SetupGet(m => m.Id).Returns(1);

            Handler = new ExecuteResultHandler(TaskService.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(new Optional<CommandInfo>(), CommandContext.Object, Result);
        }

        public ExecuteResultHandlerTestsContext SetIsNotSuccess()
        {
            Result = ExecuteResult.FromError(CommandError.Exception, "");

            TaskService
                .InSequence(Sequence)
                .Setup(s => s.Delay(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            Channel
                .InSequence(Sequence)
                .Setup(c => c.DeleteMessageAsync(It.IsAny<IMessage>(), It.IsAny<RequestOptions>()))
                .Returns(Task.CompletedTask);

            return this;
        }

        public ExecuteResultHandlerTestsContext SetIsSuccess()
        {
            Result = ExecuteResult.FromSuccess();

            return this;
        }
    }
}
