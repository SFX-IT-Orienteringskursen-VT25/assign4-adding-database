using Microsoft.EntityFrameworkCore;

public class StorageDbContext : DbContext
{
    public StorageDbContext(DbContextOptions<StorageDbContext> options)
        : base(options)
    { }

    public DbSet<StorageItem> StorageItems => Set<StorageItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StorageItem>()
            .HasKey(x => x.Key);
    }
}