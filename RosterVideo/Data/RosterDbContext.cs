using Microsoft.EntityFrameworkCore;
using RosterVideo.Models;

namespace RosterVideo.Data
{
    public class RosterDbContext : DbContext
    {
        public RosterDbContext(DbContextOptions<RosterDbContext> options) : base(options)
        {
        }

        public DbSet<RosterEntry> RosterEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RosterEntry>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                b.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                b.Property(e => e.Shortcut).IsRequired().HasMaxLength(100);
                b.Property(e => e.Major).HasMaxLength(100);
                b.Property(e => e.WhereUsed).HasMaxLength(200);
                b.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
