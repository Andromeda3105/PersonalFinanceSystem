var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

/// <summary>
/// Отримує прогноз погоди на найближчі 5 днів.
/// </summary>
/// <remarks>
/// Цей метод генерує випадкові дані. 
/// Використовується як базова перевірка працездатності API для системи особистих фінансів.
/// </remarks>
/// <returns>Список об'єктів з прогнозом погоди.</returns>
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
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

/// <summary>
/// Модель, що представляє прогноз погоди на певний день.
/// </summary>
/// <param name="Date">Дата прогнозу.</param>
/// <param name="TemperatureC">Температура у градусах Цельсія.</param>
/// <param name="Summary">Текстовий опис погодних умов.</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Температура у градусах Фаренгейта (обчислюється автоматично).
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}