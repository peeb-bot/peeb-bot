using Discord.Commands;

namespace Peeb.Bot.Results.Ok
{
    public class OkResult : RuntimeResult
    {
        public string Message { get; }

        public OkResult(string message)
            : base(null, "OK")
        {
            Message = message;
        }
    }
}
