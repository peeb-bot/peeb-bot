using System.Threading.Tasks;
using Discord.Commands;
using MediatR;
using Peeb.Bot.Pipeline.Commands.Ping;
using Peeb.Bot.Results.Ok;

namespace Peeb.Bot.Modules
{
    public class PingModule : ModuleBase<ICommandContext>
    {
        private readonly IMediator _mediator;

        public PingModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Command("ping")]
        public async Task<RuntimeResult> Ping()
        {
            var result = await _mediator.Send(new PingCommand(Context.Message.Timestamp));

            return new OkResult($"Pong! Responded in {result.Elapsed.TotalSeconds:0.000}s");
        }
    }
}
