using IT_outCRM.Blazor.Components;
using IT_outCRM.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var razorComponentsBuilder = builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Blazor Server circuit options to handle disconnections gracefully
razorComponentsBuilder.AddCircuitOptions(options =>
{
    // Increase timeout for JS interop calls to prevent premature cancellations
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    // Increase circuit disconnect timeout
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    // Increase maximum buffer size for better reliability
    options.MaxBufferedUnacknowledgedRenderBatches = 20;
});

// Get API base URL from configuration
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5295";

// Register HttpContextAccessor for cookie access
builder.Services.AddHttpContextAccessor();

// Register TokenStorage
builder.Services.AddScoped<ITokenStorage, TokenStorage>();

// Register AuthenticationHttpClientHandler
builder.Services.AddTransient<AuthenticationHttpClientHandler>();

// Configure HttpClients with authentication
// AuthService НЕ должен использовать AuthenticationHttpClientHandler!
// Login/Register не требуют токена
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<ICustomerService, CustomerService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IExecutorService, ExecutorService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<ICompanyService, CompanyService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IAccountStatusService, AccountStatusService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IOrderStatusService, OrderStatusService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IContactPersonService, ContactPersonService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IProfileService, ProfileService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60); // Больше времени для загрузки файлов
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IServiceService, ServiceService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthenticationHttpClientHandler>();

// Add Authentication with a default scheme for Blazor Server
builder.Services.AddAuthentication("BlazorAuth")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, BlazorAuthenticationHandler>("BlazorAuth", null);

// Add Authorization
// Реальная авторизация обрабатывается через CustomAuthenticationStateProvider в Blazor компонентах
// Не устанавливаем политики по умолчанию, чтобы избежать конфликтов с Blazor Server авторизацией
builder.Services.AddAuthorization();

// Register AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Add Cascading Authentication State
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // app.UseHsts(); // Отключено для разработки с HTTP backend
}
else
{
    // В режиме разработки показываем детальные ошибки
    app.UseDeveloperExceptionPage();
}

// app.UseHttpsRedirection(); // Отключено так как backend на HTTP

// Middleware для обработки favicon.ico - должен быть ДО UseStaticFiles
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path != null && path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
    {
        // Проверяем, существует ли favicon.ico
        var fileProvider = app.Environment.WebRootFileProvider;
        var icoFile = fileProvider.GetFileInfo("favicon.ico");
        
        if (icoFile.Exists)
        {
            // Если favicon.ico существует, отдаем его
            context.Response.ContentType = "image/x-icon";
            context.Response.StatusCode = 200;
            await context.Response.SendFileAsync(icoFile);
            return;
        }
        else
        {
            // Если favicon.ico не существует, отдаем favicon.png
            var pngFile = fileProvider.GetFileInfo("favicon.png");
            if (pngFile.Exists)
            {
                context.Response.ContentType = "image/png";
                context.Response.StatusCode = 200;
                await context.Response.SendFileAsync(pngFile);
                return;
            }
        }
    }
    await next();
});

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
