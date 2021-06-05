using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;
using Peeb.Bot.HostedServices;
using Peeb.Bot.MessageHandlers;
using Peeb.Bot.Settings;

namespace Peeb.Bot.UnitTests.HostedServices
{
    [TestFixture]
    [Parallelizable]
    public class BotHostedServiceTests : TestBase<BotHostedServiceTestsContext>
    {
        [Test]
        public Task StartAsync_ShouldAddSocketMessageHandler()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.VerifyAdd(sc => sc.MessageReceived += c.SocketMessageHandler.Object.MessageReceived, Times.Once));
        }

        [Test]
        public Task StartAsync_ShouldAddModulesFromEntryAssembly()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.CommandService.Verify(cs => cs.AddModulesAsync(Assembly.GetEntryAssembly(), c.ServiceProvider.Object)));
        }

        [Test]
        public Task StartAsync_ShouldLogInBot()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.LoginAsync(TokenType.Bot, c.Settings.Token, true), Times.Once()));
        }

        [Test]
        public Task StartAsync_ShouldStartDiscordSocketClient()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.StartAsync(), Times.Once()));
        }

        [Test]
        public Task StopAsync_ShouldStopDiscordSocketClient()
        {
            return TestAsync(
                c => c.StopAsync(),
                c => c.DiscordSocketClient.Verify(d => d.StopAsync(), Times.Once()));
        }
    }

    public class BotHostedServiceTestsContext
    {
        public BotHostedService BotHostedService { get; set; }
        public Mock<ICommandService> CommandService { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public Mock<ILogger<BotHostedService>> Logger { get; set; }
        public Mock<IOptionsMonitor<DiscordSettings>> OptionsMonitor { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public DiscordSettings Settings { get; set; }
        public Mock<ISocketMessageHandler> SocketMessageHandler { get; set; }

        public BotHostedServiceTestsContext()
        {
            CommandService = new Mock<ICommandService>();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Logger = new Mock<ILogger<BotHostedService>>();
            OptionsMonitor = new Mock<IOptionsMonitor<DiscordSettings>>();
            ServiceProvider = new Mock<IServiceProvider>();
            Settings = new DiscordSettings { Token = "Secret" };
            SocketMessageHandler = new Mock<ISocketMessageHandler>();

            OptionsMonitor.Setup(s => s.CurrentValue).Returns(Settings);

            BotHostedService = new BotHostedService(
                CommandService.Object,
                DiscordSocketClient.Object,
                Logger.Object,
                OptionsMonitor.Object,
                ServiceProvider.Object,
                SocketMessageHandler.Object);
        }

        public Task StartAsync()
        {
            return BotHostedService.StartAsync(CancellationToken.None);
        }

        public Task StopAsync()
        {
            return BotHostedService.StopAsync(CancellationToken.None);
        }
    }
}
