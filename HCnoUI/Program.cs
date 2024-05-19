using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck<StorageHealthCheck>("StorageHealthCheck", tags: new[] { "storage" })
    .AddCheck<DBHealthCheck>("DBHealthCheck", tags: new[] { "DB" });

var app = builder.Build();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/healthz/DB", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("DB")
});
app.MapHealthChecks("/healthz/storage", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("storage")
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
public class DBHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        string jsonFilePath = "healthDB.json";

        // Read the JSON file
        string json = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);

        // Deserialize the JSON file to a specific object type
        // Replace 'YourObjectType' with the actual type of the object you're deserializing
        HealthCheckValueClass? obj = JsonSerializer.Deserialize<HealthCheckValueClass>(json);

        // Write the HealthCheckState to the console log
        Console.WriteLine($"DBHealthCheckState: {obj?.HealthCheckState}");

        if (obj!.HealthCheckState == "Healthy")
        {
            return (HealthCheckResult.Healthy("Healthy result from DBHealthCheck"));
        }
        else if (obj!.HealthCheckState == "Degraded")
        {
            return (HealthCheckResult.Degraded("Degraded result from DBHealthCheck"));
        }

        return (HealthCheckResult.Unhealthy("Unhealthy result from DBHealthCheck"));

    }
}

public class StorageHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {        
        string jsonFilePath = "healthStorage.json";

        // Read the JSON file
        string json = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);

        // Deserialize the JSON file to a specific object type
        // Replace 'YourObjectType' with the actual type of the object you're deserializing
        HealthCheckValueClass? obj = JsonSerializer.Deserialize<HealthCheckValueClass>(json);

        // Write the HealthCheckState to the console log
        Console.WriteLine($"StorageHealthCheckState: {obj?.HealthCheckState}");

        if (obj!.HealthCheckState == "Healthy")
        {
            return (HealthCheckResult.Healthy("Healthy result from StorageHealthCheck"));
        }
        else if (obj!.HealthCheckState == "Degraded")
        {
            return (HealthCheckResult.Degraded("Degraded result from StorageHealthCheck"));
        }

        return (HealthCheckResult.Unhealthy("Unhealthy result from StorageHealthCheck"));
    }
}

public class HealthCheckValueClass
{
    public string? HealthCheckState { get; set; }
}
