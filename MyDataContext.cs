using Microsoft.EntityFrameworkCore;


public class MyDataContext : DbContext
{

    public MyDataContext(DbContextOptions <MyDataContext> options): base(options) { }

    public DbSet<PersonalInfo> Persons => Set<PersonalInfo>();
}