using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Results.Ok;

namespace Peeb.Bot.UnitTests.Results.Ok
{
    [TestFixture]
    [Parallelizable]
    public class OkResultHandlerTests : TestBase<OkResultHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldSendMessage()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        c.Result.Message,
                        false,
                        null,
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Object.Id)),
                    Times.Once));
        }
    }

    public class OkResultHandlerTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public Mock<ICommandContext> CommandContext { get; set; }
        public OkResultHandler Handler { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public OkResult Result { get; set; }

        public OkResultHandlerTestsContext()
        {
            Channel = new Mock<IMessageChannel>();
            CommandContext = new Mock<ICommandContext>();
            Message = new Mock<IUserMessage>();
            Result = new OkResult("Foobar.");

            CommandContext.SetupGet(c => c.Channel).Returns(Channel.Object);
            CommandContext.SetupGet(c => c.Message).Returns(Message.Object);
            Message.SetupGet(m => m.Id).Returns(1);

            Handler = new OkResultHandler();
        }

        public Task Handle()
        {
            return Handler.Handle(new Optional<CommandInfo>(), CommandContext.Object, Result);
        }
    }
}
