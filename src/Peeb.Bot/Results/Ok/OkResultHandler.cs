using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Clients.Discord.Handlers;

namespace Peeb.Bot.Results.Ok
{
    public class OkResultHandler : ResultHandler<OkResult>
    {
        public override Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, OkResult result)
        {
            return commandContext.Channel.SendMessageAsync(
                result.Message,
                messageReference: new MessageReference(commandContext.Message.Id));
        }
    }
}
