# 🔗 Интеграция фронтенда с бэкендом

## ✅ Что было исправлено

### 1. **Полное соответствие моделей Backend DTO**

#### AuthResponse
```csharp
// ❌ Старая версия
public class AuthResponse
{
    public Guid UserId { get; set; }  // Неправильно!
}

// ✅ Новая версия - точное соответствие backend
public class AuthResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime ExpiresAt { get; set; }  // Добавлено!
}
```

#### RegisterModel → RegisterDto
```csharp
// Добавлена конвертация UI модели в DTO
public async Task<AuthResponse?> RegisterAsync(RegisterModel model)
{
    var registerDto = new RegisterDto  // Backend DTO
    {
        Username = model.Username,
        Email = model.Email,
        Password = model.Password,  // Без ConfirmPassword!
        Role = model.Role
    };
    
    var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
}
```

### 2. **Добавлены сервисы для всех справочников**

#### Новые интерфейсы:
- ✅ `IExecutorService` - работа с исполнителями
- ✅ `ICompanyService` - работа с компаниями
- ✅ `IAccountService` - работа с аккаунтами
- ✅ `IAccountStatusService` - работа со статусами аккаунтов

#### Новые модели:
- ✅ `ExecutorModel`
- ✅ `CompanyModel`
- ✅ `AccountModel`
- ✅ `AccountStatusModel`
- ✅ `OrderStatusModel`

### 3. **Улучшенные формы с селекторами**

#### CreateOrder.razor
```razor
<!-- ❌ Старая версия -->
<InputText @bind-Value="executorIdString" />
<small>Пример: 00000000-0000-0000-0000-000000000001</small>

<!-- ✅ Новая версия -->
<InputSelect @bind-Value="CreateModel!.ExecutorId">
    <option value="">-- Выберите исполнителя --</option>
    @foreach (var executor in executors)
    {
        <option value="@executor.Id">
            @executor.AccountName (@executor.Specialization)
        </option>
    }
</InputSelect>
```

#### CreateCustomer.razor
```razor
<!-- Теперь с выбором из списков -->
<InputSelect @bind-Value="CreateModel!.AccountId">
    @foreach (var account in accounts)
    {
        <option value="@account.Id">
            @account.CompanyName (Статус: @account.AccountStatusName)
        </option>
    }
</InputSelect>

<InputSelect @bind-Value="CreateModel!.CompanyId">
    @foreach (var company in companies)
    {
        <option value="@company.Id">
            @company.Name (ИНН: @company.Inn)
        </option>
    }
</InputSelect>
```

### 4. **Асинхронная загрузка справочников**

```csharp
protected override async Task OnInitializedAsync()
{
    await LoadReferenceData();
}

private async Task LoadReferenceData()
{
    isLoadingData = true;
    
    try
    {
        var customerTask = CustomerService.GetAllAsync();
        var executorTask = ExecutorService.GetAllAsync();
        
        // Параллельная загрузка
        await Task.WhenAll(customerTask, executorTask);
        
        customers = customerTask.Result;
        executors = executorTask.Result;
    }
    catch (Exception ex)
    {
        errorMessage = $"Ошибка загрузки: {ex.Message}";
    }
    finally
    {
        isLoadingData = false;
    }
}
```

### 5. **Обновлен Program.cs**

```csharp
// Регистрация всех новых сервисов
builder.Services.AddHttpClient<IExecutorService, ExecutorService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<ICompanyService, CompanyService>(...);
builder.Services.AddHttpClient<IAccountService, AccountService>(...);
builder.Services.AddHttpClient<IAccountStatusService, AccountStatusService>(...);
```

## 🎯 Соответствие Backend API

### Controllers → Frontend Services

| Backend Controller | Frontend Service | Модель | Эндпоинты |
|-------------------|------------------|--------|-----------|
| `AuthController` | `AuthService` | `LoginModel`, `RegisterModel` | `/api/auth/login`, `/api/auth/register` |
| `OrdersController` | `OrderService` | `OrderModel`, `CreateOrderModel` | `/api/orders`, `/api/orders/paged` |
| `CustomersController` | `CustomerService` | `CustomerModel`, `CreateCustomerModel` | `/api/customers`, `/api/customers/paged` |
| `ExecutorsController` | `ExecutorService` | `ExecutorModel` | `/api/executors` |
| `CompaniesController` | `CompanyService` | `CompanyModel` | `/api/companies` |
| `AccountsController` | `AccountService` | `AccountModel` | `/api/accounts` |
| `AccountStatusesController` | `AccountStatusService` | `AccountStatusModel` | `/api/accountstatuses` |

## 📊 Поток данных

### Создание заказа:

```
1. User открывает /orders/create
2. OnInitializedAsync() загружает справочники:
   - GET /api/customers → List<CustomerModel>
   - GET /api/executors → List<ExecutorModel>
3. User заполняет форму, выбирает из списков
4. OnValidSubmit():
   - CreateOrderModel → CreateOrderDto (backend DTO)
   - POST /api/orders
5. Backend возвращает OrderDto
6. Redirect → /orders
```

### Авторизация:

```
1. User вводит логин/пароль
2. LoginModel (UI) отправляется как есть
3. POST /api/auth/login
4. Backend возвращает AuthResponseDto {
     Token,
     Username,
     Email,
     Role,
     ExpiresAt  // Новое поле!
   }
5. Token сохраняется в ProtectedLocalStorage
6. AuthenticationStateProvider обновляется
7. Все последующие запросы включают Authorization header
```

## 🔐 Авторизация запросов

Все запросы к защищенным эндпоинтам автоматически получают токен:

```csharp
// AuthenticationHttpClientHandler
protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, 
    CancellationToken cancellationToken)
{
    var token = await _tokenStorage.GetTokenAsync();
    
    if (!string.IsNullOrEmpty(token))
    {
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    return await base.SendAsync(request, cancellationToken);
}
```

## ✨ Новые возможности

### 1. Выбор клиента из списка
- Загружаются все клиенты
- Отображаются с названием аккаунта и компании
- Выбор из dropdown вместо ввода GUID

### 2. Выбор исполнителя из списка
- Загружаются все исполнители
- Отображаются с именем и специализацией
- Выбор из dropdown

### 3. Выбор аккаунта для клиента
- Загружаются все аккаунты
- Отображаются с названием компании и статусом
- Выбор из dropdown

### 4. Выбор компании для клиента
- Загружаются все компании
- Отображаются с названием и ИНН
- Выбор из dropdown

### 5. Индикаторы загрузки
```razor
@if (isLoadingData)
{
    <div class="spinner-border">Загрузка справочников...</div>
}
else
{
    <!-- Форма -->
}
```

### 6. Валидация наличия данных
```razor
<button disabled="@(!customers.Any() || !executors.Any())">
    Создать заказ
</button>

@if (!customers.Any())
{
    <div class="alert alert-warning">
        Клиенты не найдены. 
        <a href="/customers/create">Создать клиента</a>
    </div>
}
```

## 🚀 Как использовать

### 1. Подготовка данных

**Через Swagger API** (`https://localhost:7224/swagger`):

1. **Создать статус аккаунта:**
```json
POST /api/accountstatuses
{
  "name": "Активный"
}
```

2. **Создать компанию:**
```json
POST /api/companies
{
  "name": "ООО Технологии",
  "inn": "7701234567",
  "address": "г. Москва"
}
```

3. **Создать аккаунт:**
```json
POST /api/accounts
{
  "companyName": "ООО Технологии",
  "foundingDate": "2020-01-01",
  "accountStatusId": "GUID_статуса"
}
```

4. **Создать исполнителя:**
```json
POST /api/executors
{
  "accountId": "GUID_аккаунта",
  "specialization": "Backend Developer"
}
```

### 2. Работа через Blazor UI

1. **Зарегистрироваться/Войти:** `/login` или `/register`

2. **Создать клиента:**
   - `/customers/create`
   - Выбрать аккаунт из списка
   - Выбрать компанию из списка
   - Сохранить

3. **Создать заказ:**
   - `/orders/create`
   - Ввести название, описание, цену
   - Выбрать клиента из списка
   - Выбрать исполнителя из списка
   - Ввести GUID статуса (временно)
   - Сохранить

## 📝 Известные ограничения

### Временные решения:
1. **OrderStatusId** - пока вводится вручную (нет контроллера статусов заказов)
2. **SupportTeamId** - опциональное поле, пока не реализовано управление

### Требуют создания через Swagger:
- Account Statuses
- Companies
- Accounts
- Executors

## 🔮 Планы развития

### Приоритет 1:
- [ ] Создать контроллер для статусов заказов
- [ ] Добавить страницы управления компаниями
- [ ] Добавить страницы управления аккаунтами
- [ ] Добавить страницы управления исполнителями

### Приоритет 2:
- [ ] Редактирование заказов
- [ ] Редактирование клиентов
- [ ] Поиск и фильтрация во всех списках

## 🎉 Итог

**Теперь фронтенд полностью интегрирован с бэкендом:**

✅ Все модели соответствуют backend DTO
✅ Все эндпоинты вызываются правильно
✅ Формы используют селекторы вместо ввода GUID
✅ Асинхронная загрузка справочников
✅ Proper error handling
✅ Loading states
✅ Валидация данных

**Приложение готово к использованию!** 🚀

**URL:** `http://localhost:5159`




