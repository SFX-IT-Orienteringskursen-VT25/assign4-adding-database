using Microsoft.EntityFrameworkCore;
using AdditionApi.Data;
using AdditionApi.Services;
using AdditionApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Storage service for Dependency Injection
builder.Services.AddScoped<IStorageService, StorageService>();

// Add services
builder.Services.AddControllers();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<YourDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.MapControllers();

// Minimal API endpoints using Dependency Injection
app.MapPut("/storage/{key}", async (string key, StorageValue value, IStorageService storageService) =>
{
    try
    {
        await storageService.SaveValueAsync(key, value.Value);
        return Results.Ok(new { success = true, message = "Value saved successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapGet("/storage/{key}", async (string key, IStorageService storageService) =>
{
    try
    {
        var value = await storageService.GetValueAsync(key);
        if (value == null) 
            return Results.NotFound(new { success = false, message = "Key not found" });
        
        return Results.Ok(new StorageValue { Value = value });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapDelete("/storage/{key}", async (string key, IStorageService storageService) =>
{
    try
    {
        var deleted = await storageService.DeleteValueAsync(key);
        if (!deleted)
            return Results.NotFound(new { success = false, message = "Key not found" });
        
        return Results.Ok(new { success = true, message = "Value deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapGet("/storage", async (IStorageService storageService) =>
{
    try
    {
        var items = await storageService.GetAllItemsAsync();
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.Run();