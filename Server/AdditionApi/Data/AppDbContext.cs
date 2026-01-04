using AdditionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AdditionApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<StoredItem> StoredItems => Set<StoredItem>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<StoredItem>(e =>
        {
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(200);
            e.Property(x => x.ValueJson).IsRequired();
        });
    }
}
