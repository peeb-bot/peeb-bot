using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Peeb.Bot.Data;
using Peeb.Bot.Models;
using Peeb.Bot.Pipeline.Commands.SaveUser;
using Peeb.Bot.Services;

namespace Peeb.Bot.Pipeline.PreProcessors
{
    public class SaveUserPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IDateTimeOffsetService _dateTimeOffsetService;
        private readonly PeebDbContext _db;

        public SaveUserPreProcessor(IDateTimeOffsetService dateTimeOffsetService, PeebDbContext db)
        {
            _dateTimeOffsetService = dateTimeOffsetService;
            _db = db;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (request is SaveUserCommand saveUserCommand)
            {
                var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == saveUserCommand.UserId, cancellationToken);

                if (user == null)
                {
                    _db.Users.Add(new User(saveUserCommand.UserId, _dateTimeOffsetService.UtcNow));
                }

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
