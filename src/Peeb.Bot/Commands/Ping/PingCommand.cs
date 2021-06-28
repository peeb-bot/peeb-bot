using System;
using MediatR;

namespace Peeb.Bot.Commands.Ping
{
    public class PingCommand : IRequest<PingCommandResult>
    {
        public DateTimeOffset MessageTimestamp { get; }

        public PingCommand(DateTimeOffset messageTimestamp)
        {
            MessageTimestamp = messageTimestamp;
        }
    }
}
