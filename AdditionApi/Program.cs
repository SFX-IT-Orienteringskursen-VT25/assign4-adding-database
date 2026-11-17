using AdditionApi;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IDatabase, Database>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// start the MS SQL container in Docker and make sure the database and table exist
await DockerStarter.StartDockerContainerAsync();
app.Services.GetRequiredService<IDatabase>().Setup();

// localStorage.setItem(key, value)
// PUT: the client chooses the key, repeating the call gives the same result
// 204 No Content: success, nothing useful to send back
app.MapPut("/storage/{key}", (string key, [FromBody] string value, IDatabase database) =>
{
    database.SetItem(key, value);
    return Results.NoContent();
});

// localStorage.getItem(key)
// GET: read only and safe
// 200 OK + the value if it exists, 404 Not Found if not (getItem returns null)
app.MapGet("/storage/{key}", (string key, IDatabase database) =>
{
    var value = database.GetItem(key);
    return value is null ? Results.NotFound() : Results.Ok(value);
});

app.Run();
