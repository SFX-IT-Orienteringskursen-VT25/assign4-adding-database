using Microsoft.AspNetCore.Mvc;
using PersistentNumbers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

//Add Cors Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .AllowAnyOrigin() 
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//Apply the Cors policy
app.UseCors("AllowLocalhost");


//Initialise DB
Database.Setup();

app.MapGet("/", () =>
{
    return "Hello World!";
});

app.MapGet("/numbers", () =>
{
    var numbers = Database.SelectNumbers();
    return Results.Json(new { savedNumbers = numbers }, statusCode: 200);
});

app.MapPost("/numbers", (NumberInput req) =>
{
    Database.InsertValue(req.number.ToString());
    var numbers = Database.SelectNumbers();
    return Results.Json(new { savedNumbers = numbers }, statusCode: 200);
});


app.Run();

public record NumberInput(int number);
