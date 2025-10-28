# IT-outCRM.Application Layer

## 📋 Описание

Слой приложения (Application Layer) содержит всю бизнес-логику CRM-системы. Этот слой является связующим звеном между API (Presentation) и Infrastructure слоями.

## 🏗️ Структура проекта

```
IT-outCRM.Application/
├── DTOs/                      # Data Transfer Objects
│   ├── Common/
│   │   └── PagedResult.cs    # Класс для пагинации
│   ├── Account/              # DTOs для аккаунтов
│   ├── Order/                # DTOs для заказов
│   ├── Customer/             # DTOs для клиентов
│   ├── Company/              # DTOs для компаний
│   ├── Executor/             # DTOs для исполнителей
│   └── ContactPerson/        # DTOs для контактных лиц
│
├── Interfaces/               # Интерфейсы
│   ├── Repositories/        # Интерфейсы репозиториев
│   │   ├── IGenericRepository.cs
│   │   ├── IAccountRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   ├── ICompanyRepository.cs
│   │   ├── IExecutorRepository.cs
│   │   └── IContactPersonRepository.cs
│   ├── Services/            # Интерфейсы сервисов
│   │   ├── IAccountService.cs
│   │   ├── IOrderService.cs
│   │   ├── ICustomerService.cs
│   │   ├── ICompanyService.cs
│   │   ├── IExecutorService.cs
│   │   └── IContactPersonService.cs
│   └── IUnitOfWork.cs       # Unit of Work паттерн
│
├── Services/                 # Реализация бизнес-логики
│   ├── AccountService.cs
│   ├── OrderService.cs
│   ├── CustomerService.cs
│   ├── CompanyService.cs
│   ├── ExecutorService.cs
│   └── ContactPersonService.cs
│
├── Validators/               # FluentValidation валидаторы
│   ├── Account/
│   │   ├── CreateAccountValidator.cs
│   │   └── UpdateAccountValidator.cs
│   ├── Order/
│   ├── Company/
│   └── ContactPerson/
│
├── Mappings/                 # AutoMapper профили
│   ├── AccountMappingProfile.cs
│   ├── OrderMappingProfile.cs
│   ├── CustomerMappingProfile.cs
│   ├── CompanyMappingProfile.cs
│   ├── ExecutorMappingProfile.cs
│   └── ContactPersonMappingProfile.cs
│
└── DependencyInjection.cs   # Регистрация сервисов в DI
```

## 🔧 Использованные технологии

- **AutoMapper 13.0.1** - маппинг между Entity и DTO
- **FluentValidation 11.11.0** - валидация входных данных
- **FluentValidation.DependencyInjectionExtensions 11.11.0** - интеграция с DI

## 📦 Основные компоненты

### 1. DTOs (Data Transfer Objects)

Объекты для передачи данных между слоями. Каждая сущность имеет 3 типа DTO:
- `{Entity}Dto` - для чтения
- `Create{Entity}Dto` - для создания
- `Update{Entity}Dto` - для обновления

**Пример:**
```csharp
public class AccountDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public DateTime FoundingDate { get; set; }
    public Guid AccountStatusId { get; set; }
    public string AccountStatusName { get; set; }
}
```

### 2. Interfaces

#### Repositories
Определяют контракты для работы с данными:
- `IGenericRepository<T>` - базовые CRUD операции
- Специфичные репозитории с дополнительными методами

#### Services
Определяют контракты бизнес-логики:
- CRUD операции
- Пагинация
- Специфичные бизнес-методы

### 3. Services

Реализация бизнес-логики:
- Валидация бизнес-правил
- Работа с Unit of Work
- Маппинг Entity ↔ DTO
- Обработка исключений

**Пример методов:**
```csharp
Task<OrderDto?> GetByIdAsync(Guid id);
Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize);
Task<OrderDto> CreateAsync(CreateOrderDto createDto);
Task<OrderDto> UpdateAsync(UpdateOrderDto updateDto);
Task DeleteAsync(Guid id);
```

### 4. Validators

FluentValidation валидаторы для проверки входных данных:

**Пример:**
```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название заказа обязательно")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше 0");
    }
}
```

### 5. Mapping Profiles

AutoMapper профили для конвертации:

**Пример:**
```csharp
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer.Account.CompanyName));
                
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();
    }
}
```

## 🚀 Использование

### Регистрация в Program.cs

```csharp
using IT_outCRM.Application;

builder.Services.AddApplicationServices();
```

### Внедрение в контроллер

```csharp
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }
}
```

## 🎯 Паттерны и практики

- ✅ **Repository Pattern** - абстракция работы с данными
- ✅ **Unit of Work** - управление транзакциями
- ✅ **Dependency Injection** - слабая связанность
- ✅ **DTO Pattern** - разделение моделей
- ✅ **Service Layer** - изоляция бизнес-логики
- ✅ **FluentValidation** - декларативная валидация
- ✅ **AutoMapper** - автоматический маппинг

## 📝 Бизнес-правила

### Orders (Заказы)
- Цена заказа должна быть больше 0
- Название обязательно (до 200 символов)
- Проверка существования Customer и Executor

### Companies (Компании)
- Уникальный ИНН (10 или 12 цифр)
- Валидация формата ИНН
- Обязательное контактное лицо

### ContactPerson (Контактные лица)
- Уникальный email
- Валидация формата email и телефона
- Обязательные ФИО

## 🔗 Связи между слоями

```
Controllers (API) 
    ↓ использует
Services (Application)
    ↓ использует
Repositories (Infrastructure)
    ↓ работает с
Entities (Domain)
```

## 📊 Пагинация

Используется класс `PagedResult<T>`:

```csharp
public async Task<PagedResult<OrderDto>> GetOrders(int page = 1, int size = 10)
{
    return await _orderService.GetPagedAsync(page, size);
}

// Результат:
{
    "items": [...],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 15,
    "hasPreviousPage": false,
    "hasNextPage": true
}
```

## ⚡ Следующие шаги

1. Создать API контроллеры для всех сущностей
2. Добавить Swagger документацию с примерами
3. Реализовать JWT аутентификацию
4. Добавить Unit тесты для сервисов
5. Реализовать CQRS с MediatR (опционально)

