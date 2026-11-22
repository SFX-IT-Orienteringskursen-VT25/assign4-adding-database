using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("AdditionDb")
    ?? throw new InvalidOperationException("Connection string not found.");

var repository = new NumbersRepository(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/numbers", async () =>
{
    int[] numbers = await repository.GetNumbersAsync();

    return Results.Ok(numbers);
});
app.MapPut("/api/numbers", async (int[] numbers) =>
{
    await repository.SaveNumbersAsync(numbers);

    return Results.NoContent();
});

app.Run();