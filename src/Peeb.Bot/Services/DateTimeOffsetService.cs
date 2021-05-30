using System;

namespace Peeb.Bot.Services
{
    public class DateTimeOffsetService : IDateTimeOffsetService
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
