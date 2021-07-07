using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Peeb.Bot.Clients.Discord;
using Peeb.Bot.Timers;

namespace Peeb.Bot.HostedServices
{
    public class StatusHostedService : IHostedService
    {
        private static readonly List<string> Games = new()
        {
            "dead for Zenos",
            "guitar with Y'shtola",
            "WoW! jk",
            "Air Force One",
            "Any Way the Wind Blows",
            "Cliffhanger",
            "Crystal Tower Striker",
            "Cuff-a-Cur",
            "Doman Mahjong",
            "Leap of Faith",
            "Lord of Verminion",
            "Monster Toss",
            "Out on a Limb",
            "The Finer Miner",
            "The Moogle's Paw",
            "The Slice Is Right",
            "Triple Triad"
        };

        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly ITimerFactory _timerFactory;
        private ITimer _timer;
        private int _index;

        public StatusHostedService(IDiscordSocketClient discordSocketClient, ITimerFactory timerFactory)
        {
            _discordSocketClient = discordSocketClient;
            _timerFactory = timerFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = _timerFactory.CreateTimer(
                async () => await _discordSocketClient.SetGameAsync(_index == Games.Count ? Games[_index = 0] : Games[_index++]),
                TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }
    }
}
