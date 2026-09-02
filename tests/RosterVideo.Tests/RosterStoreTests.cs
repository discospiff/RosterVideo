using System.Linq;
using RosterVideo.Models;
using RosterVideo.Services;
using Xunit;

namespace RosterVideo.Tests
{
    public class RosterStoreTests
    {
        [Fact]
        public void Add_And_GetAll_ReturnsEntries()
        {
            var store = new InMemoryRosterStore();

            var entry = new RosterEntry
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Shortcut = "Ctrl+S"
            };

            store.Add(entry);

            var all = store.GetAll();

            Assert.Single(all);
            var first = all.First();
            Assert.Equal("Ada", first.FirstName);
            Assert.Equal("Lovelace", first.LastName);
            Assert.Equal("Ctrl+S", first.Shortcut);
            Assert.NotEqual(default, first.CreatedAt);
            Assert.NotEqual(default, first.Id);
        }
    }
}
