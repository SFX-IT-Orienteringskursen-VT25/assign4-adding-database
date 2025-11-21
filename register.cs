var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<StorageDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StorageDb")));

builder.Services.AddScoped<StorageService>();

var app = builder.Build();

app.MapControllers();
app.Run();
