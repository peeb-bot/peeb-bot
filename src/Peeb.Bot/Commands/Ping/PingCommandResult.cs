using System;

namespace Peeb.Bot.Commands.Ping
{
    public class PingCommandResult
    {
        public TimeSpan Elapsed { get; }

        public PingCommandResult(TimeSpan elapsed)
        {
            Elapsed = elapsed;
        }
    }
}
