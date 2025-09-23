using Microsoft.EntityFrameworkCore;
using DockerApi;
using Microsoft.EntityFrameworkCore.Storage;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

await DockerStarter.StartDockerContainerAsync();
//Database.Setup();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthorization();
app.MapControllers();



app.MapPost("/saveData", async (Item[] items, MyDbContext db) =>
{
    // Get all keys from the incoming items
    var keys = items.Select(i => i.Key).ToList();

    // Fetch existing items with the same keys
    var existingKeys = await db.Items
        .Where(i => keys.Contains(i.Key))
        .Select(i => i.Key)
        .ToListAsync();

    // Filter out duplicates
    var newItems = items
        .Where(i => !existingKeys.Contains(i.Key))
        .ToList();

    if (newItems.Count == 0)
    {
        return Results.Conflict("All items already exist.");
    }

    // Save only non-duplicates
    await db.Items.AddRangeAsync(newItems);
    await db.SaveChangesAsync();

    return Results.Created("/items", newItems);
});

app.MapGet("/items", async (MyDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});






await app.RunAsync();

