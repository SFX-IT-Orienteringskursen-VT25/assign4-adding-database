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

    await DockerStarter.StartDockerContainerAsync();

    Database.Setup();
}

app.UseHttpsRedirection();


app.MapPost("/number", (InputNumber data) =>
{

    Database.InsertValue(data.Value.ToString());
    Database.Select();
    Console.WriteLine($"Number received: {data.Value}");

    return Results.Ok(new
    {
        Message = $"Number {data.Value} was successfully added."
    });
});

app.MapGet("/numbers", () =>
{
    var rows = Database.GetAllRows();
    return Results.Ok(rows.Select(row => new { row.Id, row.Value }));
});

app.MapDelete("/numbers", () =>
{
    Database.DeleteAll();
    return Results.NoContent();
});

app.Run();