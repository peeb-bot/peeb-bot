using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Peeb.Bot.Clients.Discord.Handlers;
using Peeb.Bot.Helpers;
using Peeb.Bot.Services;

namespace Peeb.Bot.Results.Unsuccessful
{
    public class UnsuccessfulResultHandler : ResultHandler<UnsuccessfulResult>
    {
        private readonly ITaskService _taskService;

        public UnsuccessfulResultHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public override async Task Handle(Optional<CommandInfo> commandInfo, ICommandContext commandContext, UnsuccessfulResult result)
        {
            var message = await commandContext.Channel.SendMessageAsync(
                embed: new EmbedBuilder()
                    .WithColor(ColorHelper.Error)
                    .WithTitle(":(")
                    .WithDescription(result.Reason)
                    .Build(),
                messageReference: new MessageReference(commandContext.Message.Id));

            await commandContext.Channel.DeleteMessageAsync(commandContext.Message.Id);
            await _taskService.Delay(10000);
            await commandContext.Channel.DeleteMessageAsync(message);
        }
    }
}
