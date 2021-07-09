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

        private readonly IAsyncTimer _timer;
        private readonly IDiscordSocketClient _discordSocketClient;
        private int _index;

        public StatusHostedService(IAsyncTimer timer, IDiscordSocketClient discordSocketClient)
        {
            _timer = timer;
            _discordSocketClient = discordSocketClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Start(
                () => _discordSocketClient.SetGameAsync(_index == Games.Count ? Games[_index = 0] : Games[_index++]),
                TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _timer.Stop();
        }
    }
}
