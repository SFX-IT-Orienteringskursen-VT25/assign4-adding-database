using Microsoft.AspNetCore.Mvc;
using AdditionApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var storage = new Dictionary<string, string>();

app.MapGet("/", () =>
{
    return "Hello to the Addition API!";
});

app.MapGet("/addition/{key}",([FromRoute] string key) =>
{
    if(storage.TryGetValue(key, out var value))
    {
        return Results.Ok(value);
    }
    return Results.NotFound(new { Message = $"Key '{key}' not found." });
});

app.MapPost("/addition", ([FromBody] StorageRecord storageRecord) =>
{
    if(storage.ContainsKey(storageRecord.Key))
    {
        return Results.Conflict(new { Message = $"Key '{storageRecord.Key}' already exists." });
    }
    storage[storageRecord.Key] = storageRecord.Value;
    return Results.Created($"/addition/{storageRecord.Key}", storageRecord.Value);
});

app.Run();

