using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RosterVideo.Models;

namespace RosterVideo.Services
{
    public class InMemoryRosterStore : IRosterStore
    {
        private readonly List<RosterEntry> _entries = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public void Add(RosterEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            _lock.EnterWriteLock();
            try
            {
                if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
                if (entry.CreatedAt == default) entry.CreatedAt = DateTime.UtcNow;
                _entries.Add(entry);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IReadOnlyCollection<RosterEntry> GetAll()
        {
            _lock.EnterReadLock();
            try
            {
                // return a copy to avoid external mutation; callers may order as needed
                return _entries.ToList().AsReadOnly();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
