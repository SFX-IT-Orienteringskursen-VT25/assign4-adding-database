using Microsoft.AspNetCore.Mvc;
using SetupMssqlExample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Setup DB once at startup
Database.Setup();

// Serve HTML
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/Persisted-addition.html");
});

// GET: Retrieve stored numbers
app.MapGet("/api/numbers", () =>
{
    var numbers = Database.GetNumbers();
    return Results.Ok(numbers);
});

// POST: Overwrite stored numbers
app.MapPost("/api/numbers", async (HttpContext context) =>
{
    var numbers = await context.Request.ReadFromJsonAsync<List<int>>();
    if (numbers == null)
        return Results.BadRequest("Invalid JSON array");

    Database.SaveNumbers(numbers);
    return Results.NoContent();
});

app.Run();
