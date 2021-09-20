namespace Peeb.Bot.Pipeline.Commands.SaveUser
{
    public abstract class SaveUserCommand
    {
        public ulong UserId { get; }

        protected SaveUserCommand(ulong userId)
        {
            UserId = userId;
        }
    }
}
