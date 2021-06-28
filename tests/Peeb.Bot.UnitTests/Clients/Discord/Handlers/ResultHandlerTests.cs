using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Results.Ok;

namespace Peeb.Bot.UnitTests.Clients.Discord.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class ResultHandlerTests : TestBase<ResultHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldHandleResult()
        {
            return TestAsync(
                c => c.Handle(),
                c => c.Handler.Verify(h => h.Handle(It.IsAny<Optional<CommandInfo>>(), c.CommandContext.Object, c.Result.Object), Times.Once));
        }
    }

    public class ResultHandlerTestsContext
    {
        public Mock<ICommandContext> CommandContext { get; set; }
        public Mock<IResultHandler<OkResult>> Handler { get; set; }
        public Mock<IResult> Result { get; set; }

        public ResultHandlerTestsContext()
        {
            CommandContext = new Mock<ICommandContext>();
            Handler = new Mock<IResultHandler<OkResult>> { CallBase = true };
            Result = new Mock<IResult>();
        }

        public Task Handle()
        {
            return Handler.Object.Handle(new Optional<CommandInfo>(), CommandContext.Object, Result.Object);
        }
    }
}
