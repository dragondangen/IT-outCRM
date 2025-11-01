using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IT_outCRM.Middleware
{
    /// <summary>
    /// Глобальный обработчик исключений
    /// Соблюдение SOLID Single Responsibility Principle:
    /// - Middleware только обрабатывает HTTP контекст и вызывает фабрику
    /// - Создание ответов делегировано IExceptionResponseFactory
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IExceptionResponseFactory _responseFactory;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IExceptionResponseFactory responseFactory)
        {
            _next = next;
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла необработанная ошибка: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Используем фабрику для создания ответа
            var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
            var response = _responseFactory.CreateResponse(exception, environment);
            response.Path = context.Request.Path;

            context.Response.StatusCode = response.StatusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}

