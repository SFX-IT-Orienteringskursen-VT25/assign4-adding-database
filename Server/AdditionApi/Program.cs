using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdditionApi; // Namespace для YourDbContext и Storage
using AdditionApi.Models; // Namespace с моделями StorageItem и StorageValue

var builder = WebApplication.CreateBuilder(args);

// Считываем строку подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрируем контекст базы данных
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers(); // Позволяет использовать контроллеры, например WeatherForecastController

// Создаём экземпляр класса Storage для вызова методов
var storage = new Storage();

// Эндпоинты минимального API
app.MapPut("/storage/{key}", async (string key, StorageValue value, YourDbContext db) =>
{
    await storage.SaveValueAsync(key, value.Value, db);
    return Results.Ok(new { success = true });
});

app.MapGet("/storage/{key}", async (string key, YourDbContext db) =>
{
    var value = await storage.GetValueAsync(key, db);
    if (value == null) return Results.NotFound();
    return Results.Ok(new StorageValue { Value = value });
});

app.Run();
