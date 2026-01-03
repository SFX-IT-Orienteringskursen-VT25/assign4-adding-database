using AdditionApi.Services;     // <- use our services from the Services folder
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Optional: OpenAPI doc (if template had it)
builder.Services.AddOpenApi();

// Register our storage service (file-backed)
builder.Services.AddSingleton<IStorageService>(
    _ => new FileStorageService(builder.Environment.ContentRootPath));

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // optional
}

app.UseHttpsRedirection();

// Map controllers (we will put endpoints in StorageController)
app.MapControllers();

app.Run();
