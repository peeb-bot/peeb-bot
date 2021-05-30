using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Modules;
using Peeb.Bot.Services;

namespace Peeb.Bot.UnitTests.Modules
{
    [TestFixture]
    [Parallelizable]
    public class PingModuleTests : TestBase<PingModuleTestsContext>
    {
        [Test]
        public Task Ping_ShouldReplyWithPongMessage()
        {
            return TestAsync(
                c => c.Ping(),
                c => c.Channel.Verify(
                    ch => ch.SendMessageAsync(
                        "Pong! Responded in 0.543s",
                        false,
                        null,
                        null,
                        null,
                        It.Is<MessageReference>(r => r.MessageId.Value == c.Message.Id)),
                    Times.Once));
        }
    }

    public class PingModuleTestsContext
    {
        public Mock<IMessageChannel> Channel { get; set; }
        public ICommandContext Context { get; set; }
        public IDateTimeOffsetService DateTimeOffsetService { get; set; }
        public IUserMessage Message { get; set; }
        public PingModule Module { get; set; }
        public DateTimeOffset UtcNow { get; set; }

        public PingModuleTestsContext()
        {
            UtcNow = DateTimeOffset.UnixEpoch;
            Channel = new Mock<IMessageChannel>();
            Message = Mock.Of<IUserMessage>(m => m.Id == 1 && m.Timestamp == UtcNow.Subtract(TimeSpan.FromSeconds(0.5425)));
            Context = Mock.Of<ICommandContext>(c => c.Channel == Channel.Object && c.Message == Message);
            DateTimeOffsetService = Mock.Of<IDateTimeOffsetService>(s => s.UtcNow == UtcNow);

            Channel.Setup(c => c.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(Mock.Of<IUserMessage>);

            Module = new PingModule(DateTimeOffsetService).Set(m => m.Context, Context);
        }

        public Task Ping()
        {
            return Module.Ping();
        }
    }
}
