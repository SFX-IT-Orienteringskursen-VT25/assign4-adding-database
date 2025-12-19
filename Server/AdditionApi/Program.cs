using AdditionApi.Data;
using AdditionApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext (SQL Server in Docker)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Server=localhost;Database=StorageDb;User Id=sa;Password=Password123!;TrustServerCertificate=True"
    ));

var app = builder.Build();

app.UseHttpsRedirection();

// POST /storage → localStorage.setItem
app.MapPost("/storage", async (AppDbContext db, StorageItem item) =>
{
    db.StorageItems.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/storage/{item.Key}", item);
});

// GET /storage/{key} → localStorage.getItem
app.MapGet("/storage/{key}", async (AppDbContext db, string key) =>
{
    var item = await db.StorageItems.FirstOrDefaultAsync(x => x.Key == key);

    return item is null
        ? Results.NotFound(new { error = "Key not found" })
        : Results.Ok(item);
});

app.Run();
