using AdditionApi;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Addition API", Version = "v1" });
});

var credSection = builder.Configuration.GetSection("SqlCredential");
builder.Services.Configure<SqlCredential>(credSection);

builder.Services.AddSingleton<SqlDatabase>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var cred = sp.GetRequiredService<IOptions<SqlCredential>>().Value ?? new SqlCredential();
    return new SqlDatabase(cfg, cred);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
