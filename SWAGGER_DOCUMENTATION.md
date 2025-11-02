# 📚 Swagger UI - Документация API

## 🎯 Обзор

В проекте IT-outCRM интегрирована полная документация API через **Swagger UI 7.2.0** с поддержкой JWT авторизации и интерактивным тестированием всех эндпоинтов.

---

## 🚀 Быстрый старт

### 1. Запуск приложения

```bash
cd IT-outCRM
dotnet run
```

### 2. Открытие Swagger UI

После запуска откройте в браузере:

```
http://localhost:5295/swagger
```

### 3. Авторизация в Swagger

1. **Войдите в систему** через `/api/auth/login`:
   - Разверните эндпоинт `POST /api/auth/login`
   - Нажмите "Try it out"
   - Введите учетные данные:
     ```json
     {
       "username": "admin",
       "password": "Admin123!"
     }
     ```
   - Нажмите "Execute"
   - Скопируйте JWT токен из ответа

2. **Настройте авторизацию**:
   - Нажмите кнопку **"Authorize"** 🔓 в правом верхнем углу
   - В поле введите: `Bearer ваш_токен_здесь`
   - Нажмите "Authorize"
   - Закройте окно

3. **Готово!** Теперь все запросы будут выполняться с токеном авторизации

---

## ✨ Возможности Swagger UI

### Интерактивное тестирование

- ✅ **Try it out** - отправка запросов прямо из браузера
- ✅ **Автозаполнение** - примеры значений для всех параметров
- ✅ **Валидация** - проверка данных перед отправкой
- ✅ **Ответы в реальном времени** - просмотр результатов запросов

### Документация

- ✅ **Описание эндпоинтов** - подробная информация о каждом методе
- ✅ **Параметры** - типы данных, обязательность, примеры
- ✅ **Схемы данных** - структура всех DTO и моделей
- ✅ **Коды ответов** - все возможные HTTP статусы
- ✅ **Примеры запросов/ответов** - реальные примеры использования

### Навигация и поиск

- ✅ **Фильтрация** - быстрый поиск эндпоинтов
- ✅ **Группировка** - эндпоинты сгруппированы по контроллерам
- ✅ **Deep Linking** - прямые ссылки на конкретные методы
- ✅ **Время выполнения** - отображение длительности запросов

---

## 📋 Структура API

### 🔐 Аутентификация (`/api/auth`)

| Метод | Endpoint | Описание | Авторизация |
|-------|----------|----------|-------------|
| POST | `/auth/register` | Регистрация | - |
| POST | `/auth/login` | Вход (получение JWT) | - |
| GET | `/auth/me` | Текущий пользователь | ✅ Bearer |
| GET | `/auth/users` | Все пользователи | ✅ Admin |

### 📊 Аккаунты (`/api/accounts`)

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/accounts` | Все аккаунты | User+ |
| GET | `/accounts/paged` | С пагинацией | User+ |
| GET | `/accounts/{id}` | По ID | User+ |
| GET | `/accounts/by-status/{statusId}` | По статусу | User+ |
| POST | `/accounts` | Создать | Manager+ |
| PUT | `/accounts/{id}` | Обновить | Manager+ |
| DELETE | `/accounts/{id}` | Удалить | Admin |

### 📦 Заказы (`/api/orders`)

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/orders` | Все заказы | User+ |
| GET | `/orders/paged` | С пагинацией | User+ |
| GET | `/orders/{id}` | По ID | User+ |
| GET | `/orders/by-customer/{customerId}` | По клиенту | User+ |
| GET | `/orders/by-executor/{executorId}` | По исполнителю | User+ |
| GET | `/orders/by-status/{statusId}` | По статусу | User+ |
| POST | `/orders` | Создать | Manager+ |
| PUT | `/orders/{id}` | Обновить | Manager+ |
| PATCH | `/orders/{id}/status` | Изменить статус | Manager+ |
| DELETE | `/orders/{id}` | Удалить | Admin |

### 👥 Клиенты (`/api/customers`)

Полный CRUD + фильтрация по компании и аккаунту

### 🏢 Компании (`/api/companies`)

Полный CRUD + поиск по ИНН

### 👨‍💼 Исполнители (`/api/executors`)

Полный CRUD + топ исполнителей по количеству заказов

### 📞 Контактные лица (`/api/contactpersons`)

Полный CRUD + поиск по email

---

## 🔧 Технические детали

### Конфигурация Swagger

Swagger настроен в `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.3.0",
        Title = "IT-outCRM API",
        Description = "REST API для управления CRM системой",
        // ...
    });

    // JWT Security
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { /*...*/ });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { /*...*/ });

    // XML Comments
    options.IncludeXmlComments(xmlPath);
});
```

### XML Документация

В проекте включена генерация XML документации (`IT-outCRM.csproj`):

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

XML комментарии в контроллерах автоматически отображаются в Swagger UI:

```csharp
/// <summary>
/// Получить список всех аккаунтов
/// </summary>
/// <returns>Список всех аккаунтов в системе</returns>
/// <remarks>
/// Доступно для всех авторизованных пользователей (User+)
/// </remarks>
/// <response code="200">Список аккаунтов успешно получен</response>
/// <response code="401">Пользователь не авторизован</response>
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
```

### Доступные URL

| URL | Описание |
|-----|----------|
| `http://localhost:5295/swagger` | Swagger UI (интерактивная документация) |
| `http://localhost:5295/swagger/v1/swagger.json` | OpenAPI спецификация (JSON) |
| `http://localhost:5295/openapi/v1.json` | OpenAPI v1 (встроенный .NET) |

---

## 📖 Примеры использования

### Пример 1: Регистрация и вход

1. **Регистрация пользователя**:
   - Endpoint: `POST /api/auth/register`
   - Body:
     ```json
     {
       "username": "testuser",
       "email": "test@example.com",
       "password": "Test123!",
       "role": "User"
     }
     ```

2. **Вход в систему**:
   - Endpoint: `POST /api/auth/login`
   - Body:
     ```json
     {
       "username": "testuser",
       "password": "Test123!"
     }
     ```
   - Response:
     ```json
     {
       "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
       "user": {
         "id": "guid",
         "username": "testuser",
         "email": "test@example.com",
         "role": "User"
       }
     }
     ```

3. **Использование токена**:
   - Нажмите "Authorize" в Swagger
   - Введите: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

### Пример 2: Работа с аккаунтами

1. **Получить все аккаунты**:
   - Endpoint: `GET /api/accounts`
   - Требуется авторизация

2. **Получить с пагинацией**:
   - Endpoint: `GET /api/accounts/paged?pageNumber=1&pageSize=10`
   - Response:
     ```json
     {
       "items": [...],
       "totalCount": 100,
       "pageNumber": 1,
       "pageSize": 10
     }
     ```

3. **Создать новый аккаунт** (требуется роль Manager+):
   - Endpoint: `POST /api/accounts`
   - Body:
     ```json
     {
       "companyName": "ООО Технологии",
       "foundingDate": "2020-01-15",
       "accountStatusId": "guid-status-id"
     }
     ```

---

## 🎨 Настройка Swagger UI

### Кастомизация интерфейса

В `Program.cs` можно настроить вид Swagger UI:

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "IT-outCRM API v1.3.0");
    options.RoutePrefix = "swagger"; // URL: /swagger
    options.DocumentTitle = "IT-outCRM API Documentation";
    options.DefaultModelsExpandDepth(2);
    options.DefaultModelExpandDepth(2);
    options.DisplayRequestDuration(); // Показывать время запросов
    options.EnableDeepLinking(); // Deep linking
    options.EnableFilter(); // Фильтрация эндпоинтов
    options.ShowExtensions(); // Показывать расширения
});
```

### Изменение темы

Для изменения темы на темную можно добавить:

```csharp
options.InjectStylesheet("/swagger-ui/custom.css");
```

---

## 🔒 Безопасность

### JWT авторизация

- Токены генерируются при входе через `/api/auth/login`
- Срок действия токена настраивается в `appsettings.json`
- Токен передается в заголовке: `Authorization: Bearer {token}`
- Swagger UI автоматически добавляет токен ко всем запросам после авторизации

### Роли и права доступа

| Роль | Доступные операции |
|------|-------------------|
| **User** | Только чтение (GET) |
| **Manager** | Чтение, создание, обновление (GET, POST, PUT, PATCH) |
| **Admin** | Все операции включая удаление (GET, POST, PUT, PATCH, DELETE) |

---

## 🐛 Troubleshooting

### Swagger не открывается

**Проблема**: Страница `/swagger` не загружается

**Решение**:
1. Убедитесь, что приложение запущено в режиме Development
2. Проверьте, что `ASPNETCORE_ENVIRONMENT=Development`
3. Swagger доступен только в Development режиме

### Токен не работает

**Проблема**: После авторизации запросы возвращают 401

**Решение**:
1. Проверьте формат токена: должен быть `Bearer {token}`, а не просто `{token}`
2. Убедитесь, что токен не истек
3. Проверьте правильность настроек JWT в `appsettings.json`

### XML комментарии не отображаются

**Проблема**: Описания методов не видны в Swagger

**Решение**:
1. Убедитесь, что `<GenerateDocumentationFile>true</GenerateDocumentationFile>` в `.csproj`
2. Пересоберите проект: `dotnet build`
3. Проверьте наличие XML файла в `bin/Debug/net10.0/IT-outCRM.xml`

---

## 📚 Дополнительные ресурсы

- [Swagger UI Documentation](https://swagger.io/tools/swagger-ui/)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Swashbuckle.AspNetCore GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [ASP.NET Core Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

---

## ✅ Чек-лист внедрения

- ✅ Установлен пакет `Swashbuckle.AspNetCore 7.2.0`
- ✅ Настроен Swagger в `Program.cs`
- ✅ Добавлена поддержка JWT авторизации
- ✅ Включена генерация XML документации
- ✅ Добавлены XML комментарии к контроллерам
- ✅ Swagger UI доступен по адресу `/swagger`
- ✅ OpenAPI спецификация генерируется автоматически
- ✅ Настроена красивая тема и фильтрация

---

**Версия:** 1.3.1  
**Дата:** Ноябрь 2025  
**Статус:** ✅ Полностью готов к использованию

