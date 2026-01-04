using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

builder.Services.AddControllers();

var app = builder.Build();

Console.WriteLine($"ENV={builder.Environment.EnvironmentName}");
Console.WriteLine($"CS={builder.Configuration.GetConnectionString("Default")}");


app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }

