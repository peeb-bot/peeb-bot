using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Pipeline.Commands.Ping;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Pipeline.Commands.Ping
{
    [TestFixture]
    [Parallelizable]
    public class PingCommandHandlerTests : TestBase<PingCommandHandlerTestsContext>
    {
        [Test]
        public Task Handle_ShouldReturnResult()
        {
            return TestAsync(
                c => c.SetMessageTimestamp(c.UtcNow.Subtract(TimeSpan.FromSeconds(0.5425))),
                c => c.Handle(),
                (_, r) => r.Should().NotBeNull()
                    .And.BeOfType<PingCommandResult>()
                    .Which.Elapsed.Should().Be(TimeSpan.FromSeconds(0.5425)));
        }
    }

    public class PingCommandHandlerTestsContext
    {
        public Mock<IDateTimeOffsetService> DateTimeOffsetService { get; set; }
        public PingCommandHandler Handler { get; set; }
        public DateTimeOffset MessageTimestamp { get; set; }
        public DateTimeOffset UtcNow { get; set; }

        public PingCommandHandlerTestsContext()
        {
            DateTimeOffsetService = new Mock<IDateTimeOffsetService>();
            UtcNow = DateTimeOffset.UtcNow;

            DateTimeOffsetService.SetupGet(s => s.UtcNow).Returns(UtcNow);

            Handler = new PingCommandHandler(DateTimeOffsetService.Object);
        }

        public Task<PingCommandResult> Handle()
        {
            return Handler.Handle(new PingCommand(MessageTimestamp), CancellationToken.None);
        }

        public PingCommandHandlerTestsContext SetMessageTimestamp(DateTimeOffset messageTimestamp)
        {
            MessageTimestamp = messageTimestamp;

            return this;
        }
    }
}
