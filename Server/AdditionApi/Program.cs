using AdditionApi;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Загружаем .env
Env.Load();

// Читаем переменные окружения
var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? "Your_password123!";
var dbName = Environment.GetEnvironmentVariable("MSSQL_DB") ?? "MyAppDb";
var host = Environment.GetEnvironmentVariable("MSSQL_HOST") ?? "127.0.0.1";
var port = Environment.GetEnvironmentVariable("MSSQL_PORT") ?? "1433";

// Формируем строку подключения вручную
var connectionString =
    $"Server={host},{port};Database={dbName};User Id=sa;Password={saPassword};TrustServerCertificate=True;";

// Записываем строку подключения в конфигурацию
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// Подключаем сервисы
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine(">>> ENV is Development, starting Docker container...");
    await DockerStarter.EnsureSqlContainerAsync(app.Configuration);
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
