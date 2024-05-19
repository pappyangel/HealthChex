using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();
builder.Services.AddHealthChecks()        
    .AddCheck<CustomHealthCheck>("CustomHealthCheck", tags: new[] { "custom" })
    .AddCheck<DBHealthCheck>("DBHealthCheck", tags: new[] { "DB" });

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();

app.MapHealthChecksUI();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/health/custom", new HealthCheckOptions
{
    Predicate = reg => reg.Tags.Contains("custom"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
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
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Random _random = new Random();
        var responseTime = _random.Next(1, 300);
        if (responseTime < 100)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Healthy result from MyHealthCheck"));
        }
        else if (responseTime < 200)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Degraded result from MyHealthCheck"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from MyHealthCheck"));
    }
}

public class DBHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Random _random = new Random();
        var responseTime = _random.Next(1, 300);
        if (responseTime < 100)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Healthy result from MyHealthCheck"));
        }
        else if (responseTime < 200)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Degraded result from MyHealthCheck"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from MyHealthCheck"));
    }
}