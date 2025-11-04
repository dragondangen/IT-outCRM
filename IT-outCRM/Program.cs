using System.Text;
using System.Reflection;
using System.Threading.RateLimiting;
using IT_outCRM.Application;
using IT_outCRM.Infrastructure;
using IT_outCRM.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов инфраструктуры и приложения
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Middleware сервисы (SRP - разделение ответственностей)
builder.Services.AddSingleton<IExceptionResponseFactory, ExceptionResponseFactory>();

// Конфигурация JWT аутентификации
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

// FluentValidation автоматическая валидация
builder.Services.AddFluentValidationAutoValidation();

// Rate Limiting - защита от brute-force атак
builder.Services.AddRateLimiter(options =>
{
    // Политика для аутентификации (login/register)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5; // 5 попыток
        limiterOptions.Window = TimeSpan.FromMinutes(1); // за 1 минуту
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2; // макс 2 в очереди
    });

    // Общая политика для API (защита от DoS)
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100; // 100 запросов
        limiterOptions.Window = TimeSpan.FromMinutes(1); // за 1 минуту
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Сообщение при превышении лимита
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429; // Too Many Requests
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Слишком много запросов",
            message = "Вы превысили лимит запросов. Пожалуйста, попробуйте позже.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
                ? retryAfter.ToString() 
                : "60 секунд"
        }, cancellationToken: token);
    };
});

// CORS - настройка для Development и Production
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development: разрешить все источники для удобства разработки
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
    else
    {
        // Production: ограничить конкретными доменами
        options.AddDefaultPolicy(policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? new[] { "https://yourdomain.com" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    }
});

// Swagger конфигурация
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.3.2",
        Title = "IT-outCRM API",
        Description = "REST API для управления CRM системой IT-аутсорсинговой компании. " +
                      "Поддерживает управление клиентами, заказами, исполнителями, компаниями, контактными лицами и статусами аккаунтов.",
        Contact = new OpenApiContact
        {
            Name = "IT-outCRM Support",
            Email = "support@it-outcrm.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // JWT авторизация в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                      "Введите 'Bearer' [пробел] и затем ваш токен.\r\n\r\n" +
                      "Пример: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Включение XML комментариев
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Глобальный обработчик исключений
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Инициализация БД - применение миграций для всех окружений
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    // Migrate() работает для Development и Production
    // Создает БД если не существует и применяет все миграции
    context.Database.Migrate();
}

// Swagger только для Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IT-outCRM API v1.3.0");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "IT-outCRM API Documentation";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

// HSTS для Production (HTTPS принудительно)
if (app.Environment.IsProduction())
{
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection();

// Rate Limiting перед CORS
app.UseRateLimiter();

app.UseCors();

// Важно: аутентификация перед авторизацией
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();