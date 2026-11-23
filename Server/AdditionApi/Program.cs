using System.ComponentModel.DataAnnotations;
using AdditionApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StorageDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

await DockerStarter.StartDockerContainerAsync("YourStrong!Password123");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StorageDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPut("/storage/{key}", async (string key, [FromBody] string value, StorageDbContext db) =>
{
    var item = await db.StorageItems.FindAsync(key);
    if (item is null)
    {
        db.StorageItems.Add(new StorageItem { Key = key, Value = value });
    }
    else
    {
        item.Value = value;
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/storage/{key}", async (string key, StorageDbContext db) =>
{
    var item = await db.StorageItems.FindAsync(key);
    return item is not null
        ? Results.Ok(item.Value)
        : Results.NotFound();
});

app.Run();

public class StorageItem
{
    [Key]
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class StorageDbContext : DbContext
{
    public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options) { }
    public DbSet<StorageItem> StorageItems => Set<StorageItem>();
}