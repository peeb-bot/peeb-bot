using System.Text;
using Discord.Commands;

namespace Peeb.Bot.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendCommandContext(this StringBuilder stringBuilder, ICommandContext commandContext)
        {
            return stringBuilder
                .AppendFormat("command '{0}' ", commandContext.Message.Content)
                .AppendFormat("on server '{0}/{1}' ", commandContext.Guild.Id, commandContext.Guild.Name)
                .AppendFormat("in channel '{0}/{1}' ", commandContext.Channel.Id, commandContext.Channel.Name)
                .AppendFormat("for user '{0}/{1}#{2}'", commandContext.User.Id, commandContext.User.Username, commandContext.User.Discriminator);
        }
    }
}
