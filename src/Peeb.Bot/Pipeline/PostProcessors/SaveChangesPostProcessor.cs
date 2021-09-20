using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Peeb.Bot.Data;
using Peeb.Bot.Models;

namespace Peeb.Bot.Pipeline.PostProcessors
{
    public class SaveChangesPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly IMediator _mediator;
        private readonly PeebDbContext _db;

        public SaveChangesPostProcessor(IMediator mediator, PeebDbContext db)
        {
            _mediator = mediator;
            _db = db;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            List<INotificationSource> notificationSources;

            while ((notificationSources = _db
                    .ChangeTracker
                    .Entries<INotificationSource>()
                    .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                    .Select(e => e.Entity)
                    .ToList())
                .Any())
            {
                await _db.SaveChangesAsync(cancellationToken);

                foreach (var notification in notificationSources.SelectMany(s => s.Flush()).ToList())
                {
                    await _mediator.Publish(notification, cancellationToken);
                }
            }
        }
    }
}
