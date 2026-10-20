using DAL.Entities;
using DAL.Repositories.IRepo;
using DAL.Repositories.Repo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL / Entity Framework
builder.Services.AddDbContext<STKDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

// Test endpoint
app.MapGet("/", () => new
{
    message = "Gestion Stock API",
    status = "API is running"
});

// Test PostgreSQL connection
app.MapGet("/api/test-db", async (STKDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();

        return Results.Ok(new
        {
            database = "Gestion_stk",
            connected = canConnect
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();