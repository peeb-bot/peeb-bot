using System;

namespace Peeb.Bot.Services
{
    public interface IDateTimeOffsetService
    {
        DateTimeOffset UtcNow { get; }
    }
}
