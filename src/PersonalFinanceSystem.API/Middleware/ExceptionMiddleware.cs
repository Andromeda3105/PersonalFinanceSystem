using System.Net;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceSystem.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Генеруємо унікальний ID помилки для кореляції логів та відповіді користувачу
            var errorId = Guid.NewGuid().ToString();

            // Логуємо детальну інформацію про помилку разом із ID
            _logger.LogError(ex, "Помилка {ErrorId}: {Message}. Шлях: {Path}",
                errorId, ex.Message, context.Request.Path);

            await HandleExceptionAsync(context, ex, errorId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string errorId)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Формуємо об'єкт відповіді для клієнта
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "На сервері сталася непередбачувана помилка. Наші фахівці вже працюють над її вирішенням.",
            ErrorId = errorId,
            Detailed = "Зверніться до адміністратора, вказавши унікальний ідентифікатор помилки для діагностики."
        };

        // Налаштування серіалізації: CamelCase назви полів та підтримка кирилиці
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}