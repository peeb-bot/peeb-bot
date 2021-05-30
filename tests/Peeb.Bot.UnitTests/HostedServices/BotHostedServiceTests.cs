using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Discord.Commands;
using Peeb.Bot.Discord.WebSocket;
using Peeb.Bot.HostedServices;
using Peeb.Bot.MessageHandlers;

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
                c => c.DiscordSocketClient.Verify(sc => sc.LoginAsync(TokenType.Bot, c.DiscordBotToken, true), Times.Once()));
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
        public Mock<ICommandService> CommandService { get; set; }
        public Mock<IConfiguration> Configuration { get; set; }
        public string DiscordBotToken { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public Mock<ILogger<BotHostedService>> Logger { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public Mock<ISocketMessageHandler> SocketMessageHandler { get; set; }
        public BotHostedService BotHostedService { get; set; }

        public BotHostedServiceTestsContext()
        {
            CommandService = new Mock<ICommandService>();
            Configuration = new Mock<IConfiguration>();
            DiscordBotToken = "ABC123";
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            Logger = new Mock<ILogger<BotHostedService>>();
            ServiceProvider = new Mock<IServiceProvider>();
            SocketMessageHandler = new Mock<ISocketMessageHandler>();

            Configuration.Setup(c => c["DISCORD_BOT_TOKEN"]).Returns(DiscordBotToken);

            BotHostedService = new BotHostedService(
                CommandService.Object,
                Configuration.Object,
                DiscordSocketClient.Object,
                Logger.Object,
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
