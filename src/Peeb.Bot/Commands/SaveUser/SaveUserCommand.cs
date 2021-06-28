namespace Peeb.Bot.Commands.SaveUser
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
