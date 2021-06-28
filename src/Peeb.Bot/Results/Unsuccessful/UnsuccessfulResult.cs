using Discord.Commands;

namespace Peeb.Bot.Results.Unsuccessful
{
    public class UnsuccessfulResult : RuntimeResult
    {
        public UnsuccessfulResult(string reason)
            : base(CommandError.Unsuccessful, reason)
        {
        }
    }
}
