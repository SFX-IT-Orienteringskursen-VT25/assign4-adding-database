using Microsoft.EntityFrameworkCore;
using assign3_addition_api.Models;  // <-- This is required

namespace assign3_addition_api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<StorageItem> StorageItems { get; set; } = null!;
    }
}
