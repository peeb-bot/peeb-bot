using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Clients.Discord.Services;
using Peeb.Bot.HostedServices;
using Peeb.Bot.Settings;

namespace Peeb.Bot.UnitTests.HostedServices
{
    [TestFixture]
    [Parallelizable]
    public class DiscordHostedServiceTests : TestBase<DiscordHostedServiceTestsContext>
    {
        [Test]
        public Task StartAsync_ShouldAddMessageReceivedHandler()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.VerifyAdd(sc => sc.MessageReceived += c.MessageHandler.Object.MessageReceived, Times.Once));
        }

        [Test]
        public Task StartAsync_ShouldAddCommandExecutedHandler()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.CommandService.VerifyAdd(sc => sc.CommandExecuted += c.MessageHandler.Object.CommandExecuted, Times.Once));
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
                c => c.DiscordSocketClient.Verify(sc => sc.LoginAsync(TokenType.Bot, c.Settings.Token, true), Times.Once));
        }

        [Test]
        public Task StartAsync_ShouldStartDiscordSocketClient()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.StartAsync(), Times.Once));
        }

        [Test]
        public Task StartAsync_ShouldSetGame()
        {
            return TestAsync(
                c => c.StartAsync(),
                c => c.DiscordSocketClient.Verify(sc => sc.SetGameAsync("Final Fantasy XIV", null, ActivityType.Playing), Times.Once));
        }

        [Test]
        public Task StopAsync_ShouldStopDiscordSocketClient()
        {
            return TestAsync(
                c => c.StopAsync(),
                c => c.DiscordSocketClient.Verify(d => d.StopAsync(), Times.Once));
        }
    }

    public class DiscordHostedServiceTestsContext
    {
        public Mock<ICommandService> CommandService { get; set; }
        public Mock<IDiscordSocketClient> DiscordSocketClient { get; set; }
        public DiscordHostedService HostedService { get; set; }
        public Mock<IMessageHandler> MessageHandler { get; set; }
        public Mock<IOptionsMonitor<DiscordSettings>> OptionsMonitor { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public DiscordSettings Settings { get; set; }

        public DiscordHostedServiceTestsContext()
        {
            CommandService = new Mock<ICommandService>();
            DiscordSocketClient = new Mock<IDiscordSocketClient>();
            MessageHandler = new Mock<IMessageHandler>();
            OptionsMonitor = new Mock<IOptionsMonitor<DiscordSettings>>();
            ServiceProvider = new Mock<IServiceProvider>();
            Settings = new DiscordSettings { Token = "Secret" };

            OptionsMonitor.SetupGet(m => m.CurrentValue).Returns(Settings);

            HostedService = new DiscordHostedService(
                CommandService.Object,
                DiscordSocketClient.Object,
                MessageHandler.Object,
                OptionsMonitor.Object,
                ServiceProvider.Object);
        }

        public Task StartAsync()
        {
            return HostedService.StartAsync(CancellationToken.None);
        }

        public Task StopAsync()
        {
            return HostedService.StopAsync(CancellationToken.None);
        }
    }
}
