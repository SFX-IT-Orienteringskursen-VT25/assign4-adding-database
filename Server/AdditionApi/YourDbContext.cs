using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;
public class YourDbContext : DbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options) : base(options) {}

    public DbSet<StorageItem> StorageItems { get; set; }
}
