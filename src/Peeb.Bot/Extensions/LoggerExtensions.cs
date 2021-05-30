using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Peeb.Bot.Extensions
{
    public static class LoggerExtensions
    {
        public static Task Log(this ILogger logger, LogMessage logMessage)
        {
            logger.Log(
                (LogLevel)Math.Abs((int)logMessage.Severity - 5),
                0,
                logMessage,
                logMessage.Exception,
                (_, _) => logMessage.ToString(prependTimestamp: false));

            return Task.CompletedTask;
        }
    }
}
