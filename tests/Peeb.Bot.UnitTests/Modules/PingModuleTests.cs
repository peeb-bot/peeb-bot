using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Modules;
using Peeb.Bot.Pipeline.Commands.Ping;
using Peeb.Bot.Results.Ok;

namespace Peeb.Bot.UnitTests.Modules
{
    [TestFixture]
    [Parallelizable]
    public class PingModuleTests : TestBase<PingModuleTestsContext>
    {
        [Test]
        public Task Ping_ShouldSendCommand()
        {
            return TestAsync(
                c => c.Ping(),
                c => c.Mediator.Verify(m => m.Send(It.Is<PingCommand>(cm => cm.MessageTimestamp == c.Timestamp), CancellationToken.None)));
        }

        [Test]
        public Task Ping_ShouldReturnOkResult()
        {
            return TestAsync(
                c => c.Ping(),
                (c, r) => r.Should().NotBeNull()
                    .And.BeOfType<OkResult>()
                    .Which.Message.Should().Be("Pong! Responded in 0.543s"));
        }
    }

    public class PingModuleTestsContext
    {
        public Mock<ICommandContext> Context { get; set; }
        public TimeSpan Elapsed { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IUserMessage> Message { get; set; }
        public PingModule Module { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public PingModuleTestsContext()
        {
            Context = new Mock<ICommandContext>();
            Timestamp = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(0.5425));
            Elapsed = DateTimeOffset.UtcNow.Subtract(Timestamp);
            Mediator = new Mock<IMediator>();
            Message = new Mock<IUserMessage>();

            Mediator.Setup(m => m.Send(It.IsAny<PingCommand>(), CancellationToken.None)).ReturnsAsync(new PingCommandResult(Elapsed));
            Message.SetupGet(m => m.Timestamp).Returns(Timestamp);
            Context.SetupGet(c => c.Message).Returns(Message.Object);

            Module = new PingModule(Mediator.Object).Set(m => m.Context, Context.Object);
        }

        public Task<RuntimeResult> Ping()
        {
            return Module.Ping();
        }
    }
}
