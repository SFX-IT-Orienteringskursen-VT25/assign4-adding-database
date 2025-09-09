using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;

namespace AdditionApi.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options) { }

        public DbSet<StorageItem> StorageItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Make Key unique to prevent duplicate keys
            modelBuilder.Entity<StorageItem>()
                .HasIndex(s => s.Key)
                .IsUnique();

            // Configure Key column
            modelBuilder.Entity<StorageItem>()
                .Property(s => s.Key)
                .HasMaxLength(255)
                .IsRequired();

            // Configure Value column
            modelBuilder.Entity<StorageItem>()
                .Property(s => s.Value)
                .IsRequired();
        }
    }
}