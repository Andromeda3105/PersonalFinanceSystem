using Serilog;

using PersonalFinanceSystem.API.Middleware;

// Налаштування початкового логування Serilog (до запуску хоста)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/finance-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Інтеграція Serilog у систему логування ASP.NET Core
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Додавання сервісів у контейнер (Swagger/OpenAPI)
builder.Services.AddOpenApi();

try
{
    Log.Information("Запуск застосунку Personal Finance API...");

    var app = builder.Build();

    // Реєстрація кастомного Middleware для глобальної обробки помилок
    app.UseMiddleware<ExceptionMiddleware>();

    // Налаштування конвеєра HTTP-запитів
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    // Масив даних для тестового ендпоінту
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Отримує прогноз погоди на найближчі 5 днів.
    /// Використовується для діагностики працездатності API.
    /// </summary>
    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        Log.Information("Запит на прогноз погоди успішно оброблено.");
        return forecast;
    })
    .WithName("GetWeatherForecast");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Критична помилка: Застосунок не зміг запуститися!");
}
finally
{
    Log.Information("Зупинка застосунку Personal Finance API...");
    Log.CloseAndFlush();
}

/// <summary>
/// Модель даних для прогнозу погоди.
/// </summary>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}