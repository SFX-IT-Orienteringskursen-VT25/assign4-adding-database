using Microsoft.EntityFrameworkCore;
using DockerWebApi;
using Microsoft.EntityFrameworkCore.Storage;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDataContext>(options =>
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



app.MapPost("/saveData", async (PersonalInfo person, MyDataContext db) =>
{

    db.Persons.Add(person);
    await db.SaveChangesAsync();

    return Results.Created($"/save/{person.Id}", person);
});

app.MapGet("/GetAllPerson", async (MyDataContext db) =>
{
    var personalDetails = await db.Persons.ToListAsync();
    return Results.Ok(personalDetails);
});






await app.RunAsync();

