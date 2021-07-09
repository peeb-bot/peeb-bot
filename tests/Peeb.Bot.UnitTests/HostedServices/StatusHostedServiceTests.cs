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
        public Task StartAsync_ShouldStartTimer()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.Timer.Verify(t => t.Start(It.IsAny<Func<Task>>(), TimeSpan.Zero, TimeSpan.FromHours(1)), Times.Once));
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
        public Task StopAsync_ShouldStopTimer()
        {
            return TestAsync(
                c => c.StopAsync(),
                c => c.Timer.Verify(t => t.Stop(), Times.Once));
        }
    }

    public class StatusHostedServiceTestsContext
    {
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public StatusHostedService HostedService { get; set; }
        public Mock<IAsyncTimer> Timer { get; set; }

        public StatusHostedServiceTestsContext()
        {
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Timer = new Mock<IAsyncTimer>();

            Timer.SetupGet(t => t.Elapsed).Returns(false);

            HostedService = new StatusHostedService(Timer.Object, DiscordSocketClient.Object);
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
            Timer
                .Setup(t => t.Start(It.IsAny<Func<Task>>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Func<Task> callback, TimeSpan _, TimeSpan _) => callback());

            Timer.SetupGet(t => t.Elapsed).Returns(true);

            return this;
        }

        public StatusHostedServiceTestsContext SetPeriodElapsed()
        {
            Timer
                .Setup(t => t.Start(It.IsAny<Func<Task>>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback(async (Func<Task> callback, TimeSpan _, TimeSpan _) =>
                {
                    await callback();
                    await callback();
                });

            Timer.SetupGet(t => t.Elapsed).Returns(true);

            return this;
        }
    }
}
