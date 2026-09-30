using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RosterVideo.Data;
using RosterVideo.Models;

namespace RosterVideo.Services
{
    public class DbRosterStore : IRosterStore
    {
        private readonly RosterDbContext _db;
        private readonly ILogger<DbRosterStore> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public DbRosterStore(RosterDbContext db, ILogger<DbRosterStore> logger)
        {
            _db = db;
            _logger = logger;

            // Simple retry policy: 3 retries, exponential backoff with jitter
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                {
                    var delay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt - 1));
                    // add small jitter
                    var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 100));
                    return delay + jitter;
                }, (ex, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(ex, "DbRosterStore.Add retry {RetryCount} after {Delay} due to error", retryCount, timespan);
                });
        }

        public void Add(RosterEntry entry)
        {
            // Fire and forget synchronous wrapper
            AddAsync(entry).GetAwaiter().GetResult();
        }

        public IReadOnlyCollection<RosterEntry> GetAll()
        {
            var list = _db.RosterEntries.AsNoTracking().OrderByDescending(e => e.CreatedAt).ToList();
            return list.AsReadOnly();
        }

        private async Task AddAsync(RosterEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
            if (entry.CreatedAt == default) entry.CreatedAt = DateTime.UtcNow;

            await _retryPolicy.ExecuteAsync(async () =>
            {
                _db.RosterEntries.Add(entry);
                await _db.SaveChangesAsync();
            });
        }
    }
}
