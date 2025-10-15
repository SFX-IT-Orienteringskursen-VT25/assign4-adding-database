using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;

namespace AdditionApi.Data
{
    public class NumbersContext : DbContext
    {
        public NumbersContext(DbContextOptions<NumbersContext> options) : base(options) { }
        public DbSet<Number> Numbers { get; set; }
    }
}
