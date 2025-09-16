using AdditionApi;
using Microsoft.AspNetCore.Mvc;

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


app.MapPost("/addNumber", (InputNumber data) =>
{
    Console.WriteLine($"Number received: {data.Value}");

    return Results.Ok(new
    {
        Message = $"Number {data.Value} was successfully added."
    });
});

app.MapGet("/numbers", () =>
{
    int[] numbers = { 10, 20, 30 };
    Console.WriteLine("Returning multiple numbers");
    return Results.Ok(numbers);
});

app.Run();