using GameStore.api.Data;
using GameStore.api.Endpoints;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DB Context setup
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GamesStoreContext>(connString);

// ✅ Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GameStore API",
        Version = "v1",
        Description = "A minimal API for managing games and genres"
    });
});

var app = builder.Build();

// ✅ Enable Swagger in all environments (or conditionally in dev)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore API v1");
    options.RoutePrefix = string.Empty; // to serve at root http://localhost:port/
});

// Register endpoints
app.MapGamesEndPoints();
app.MapGenresEndPoints();

// Migrate DB
await app.MigrateDbAsync();

app.Run();
