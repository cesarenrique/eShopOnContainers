using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CatalogContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




/*
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<CatalogContext>();
        //it creates the DB in case it does not exist
        //this is used only while developing the system
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}
*/
// la conexión a la bd está definida en appsettings
string? db2Use = Environment.GetEnvironmentVariable("DB2USE");
// En caso de que el entorno de despliegue producción (Docker)
if (db2Use == "DockerSQLServer")
{
    builder.Services.AddDbContext<CatalogContext>(options => {
        options.UseSqlServer(builder.Configuration
        .GetConnectionString("SQLServerDockerConnection"),
        sqlServerOptionsAction: sqlOptions => {
            //Recuperamos el nombre del Ensamblado donde está la migración
            sqlOptions.MigrationsAssembly(
    Assembly.GetExecutingAssembly().GetName().Name);
            //Configuramos una conexión resiliente a la BD
            sqlOptions.
    EnableRetryOnFailure(maxRetryCount: 5,
    maxRetryDelay: TimeSpan.FromSeconds(30),
    errorNumbersToAdd: null);
        });
    });
}
else
{
    builder.Services.AddDbContext<CatalogContext>(options => options
    .UseSqlServer(builder.Configuration
    .GetConnectionString("DefaultConnection")));
}

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
