using System.Threading.Tasks;
using Discord.Commands;
using MediatR;
using Peeb.Bot.Commands.SaveCharacter;
using Peeb.Bot.Results.Character;
using Peeb.Bot.Results.Unsuccessful;

namespace Peeb.Bot.Modules
{
    public class CharacterModule : ModuleBase<ICommandContext>
    {
        private readonly IMediator _mediator;

        public CharacterModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Command("iam")]
        public async Task<RuntimeResult> IAm(string world, string forename, string surname)
        {
            var result = await _mediator.Send(new SaveCharacterCommand(Context.User.Id, Context.Guild.Id, world, forename, surname));

            return result.Character != null
                ? new IAmResult(result.Character)
                : new UnsuccessfulResult("Sorry, I couldn't find your character. Please check your command and try again.");
        }
    }
}
