using EFSearch.Sample.Api.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting EFSearch Sample API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Serialize/deserialize enums as strings, e.g., "Equals" instead of 0
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // Configure EF Core with configurable database provider
    var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "InMemory";

    builder.Services.AddDbContext<SampleDbContext>(options =>
    {
        if (databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite(connectionString);
            Log.Information("Using SQLite database - SQL queries will be logged");
        }
        else
        {
            options.UseInMemoryDatabase("SampleDb");
            Log.Information("Using InMemory database - Query details will be logged");
        }
        
        // Enable sensitive data logging and detailed errors for development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(message => Log.Debug(message), new[] { 
                Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting,
                Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.QueryCompilationStarting,
                Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ContextInitialized
            }, LogLevel.Information);
        }
    });

    var app = builder.Build();

    // Use Serilog for request logging
    app.UseSerilogRequestLogging();

    // Seed the database
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
        
        if (databaseProvider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Log.Information("SQLite database created and seeded");
        }
        else
        {
            dbContext.Database.EnsureCreated();
        }
    }

    // Configure the HTTP request pipeline.
    app.UseHttpsRedirection();

    app.MapControllers();

    Log.Information("EFSearch Sample API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
