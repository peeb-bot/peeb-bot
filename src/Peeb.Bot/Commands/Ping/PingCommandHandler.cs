using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Peeb.Bot.Services;

namespace Peeb.Bot.Commands.Ping
{
    public class PingCommandHandler : IRequestHandler<PingCommand, PingCommandResult>
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;

        public PingCommandHandler(IDateTimeOffsetService dateTimeOffsetService)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
        }

        public Task<PingCommandResult> Handle(PingCommand request, CancellationToken cancellationToken)
        {
            var elapsed = _dateTimeOffsetService.UtcNow.Subtract(request.MessageTimestamp);

            return Task.FromResult(new PingCommandResult(elapsed));
        }
    }
}
