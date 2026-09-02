using System.Collections.Generic;
using RosterVideo.Models;

namespace RosterVideo.Services
{
    public interface IRosterStore
    {
        void Add(RosterEntry entry);
        IReadOnlyCollection<RosterEntry> GetAll();
    }
}
