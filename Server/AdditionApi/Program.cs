using AdditionApi.Data;
using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();

// Add DbContext for SQL Server
builder.Services.AddDbContext<NumbersContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=AdditionDb;User Id=sa;Password=YourStrong@Passw0rd1;TrustServerCertificate=True;"));
//  Add controller services
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



// Map controller routes
app.MapControllers();

app.Run();