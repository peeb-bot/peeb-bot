using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.HostedServices;
using Peeb.Bot.Timers;

namespace Peeb.Bot.UnitTests.HostedServices
{
    [TestFixture]
    [Parallelizable]
    public class StatusHostedServiceTests : TestBase<StatusHostedServiceTestsContext>
    {
        [Test]
        public Task StartAsync_ShouldCreateTimer()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.TimerFactory.Verify(f => f.CreateTimer(It.IsAny<Action>(), TimeSpan.Zero, TimeSpan.FromHours(1)), Times.Once));
        }

        [Test]
        public Task StartAsync_DueTimeElapsed_ShouldSetStatus()
        {
            return TestAsync(
                c => c.SetDueTimeElapsed(),
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.SetGameAsync("dead for Zenos", null, ActivityType.Playing), Times.Once));
        }

        [Test]
        public Task StartAsync_DueTimeElapsedAndPeriodElapsed_ShouldUpdateStatus()
        {
            return TestAsync(
                c => c.SetPeriodElapsed(),
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.SetGameAsync("guitar with Y'shtola", null, ActivityType.Playing), Times.Once));
        }

        [Test]
        public Task StopAsync_ShouldDisposeTimer()
        {
            return TestAsync(
                c => c.StopAsync(),
                c => c.Timer.Verify(t => t.DisposeAsync(), Times.Once));
        }
    }

    public class StatusHostedServiceTestsContext
    {
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public StatusHostedService HostedService { get; set; }
        public Mock<ITimer> Timer { get; set; }
        public Mock<ITimerFactory> TimerFactory { get; set; }

        public StatusHostedServiceTestsContext()
        {
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Timer = new Mock<ITimer>();
            TimerFactory = new Mock<ITimerFactory>();

            Timer.SetupGet(t => t.Elapsed).Returns(false);
            TimerFactory.Setup(f => f.CreateTimer(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(Timer.Object);

            HostedService = new StatusHostedService(DiscordSocketClient.Object, TimerFactory.Object);
        }

        public Task StartAsync()
        {
            return HostedService.StartAsync(CancellationToken.None);
        }

        public async Task StopAsync()
        {
            await StartAsync();
            await HostedService.StopAsync(CancellationToken.None);
        }

        public StatusHostedServiceTestsContext SetDueTimeElapsed()
        {
            Timer.SetupGet(t => t.Elapsed).Returns(true);

            TimerFactory
                .Setup(f => f.CreateTimer(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Action callback, TimeSpan _, TimeSpan _) => callback())
                .Returns(Timer.Object);

            return this;
        }

        public StatusHostedServiceTestsContext SetPeriodElapsed()
        {
            Timer.SetupGet(t => t.Elapsed).Returns(true);

            TimerFactory
                .Setup(f => f.CreateTimer(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Action callback, TimeSpan _, TimeSpan _) =>
                {
                    callback();
                    callback();
                })
                .Returns(Timer.Object);

            return this;
        }
    }
}
