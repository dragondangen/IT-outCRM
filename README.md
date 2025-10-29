# 🚀 IT-outCRM - CRM система для IT-аутсорсинга

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue.svg)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-blue.svg)](https://docs.microsoft.com/en-us/ef/core/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**IT-outCRM** - это современная CRM система для управления клиентами, заказами и исполнителями в IT-аутсорсинговой компании. Построена на основе Clean Architecture с использованием последних версий .NET и PostgreSQL.

---

## 📋 Содержание

- [О проекте](#-о-проекте)
- [Архитектура](#-архитектура)
- [Технологии](#-технологии)
- [Функциональность](#-функциональность)
- [Установка](#-установка)
- [Запуск](#-запуск)
- [API](#-api)
- [Структура проекта](#-структура-проекта)
- [База данных](#-база-данных)
- [Последние обновления](#-последние-обновления)
- [Разработка](#-разработка)
- [Лицензия](#-лицензия)

---

## 🎯 О проекте

IT-outCRM - это полнофункциональная CRM система, предназначенная для управления бизнес-процессами IT-аутсорсинговых компаний. Система предоставляет REST API для работы с клиентами, заказами, исполнителями и компаниями.

### Ключевые особенности:

- ✅ **Clean Architecture** - четкое разделение слоёв и зависимостей
- ✅ **JWT Authentication** - безопасная аутентификация с ролями
- ✅ **RESTful API** - 52 эндпоинта для всех операций
- ✅ **Entity Framework Core** - современный ORM с миграциями
- ✅ **FluentValidation** - декларативная валидация данных
- ✅ **AutoMapper** - автоматический маппинг между моделями
- ✅ **PostgreSQL** - надёжная СУБД с поддержкой Docker
- ✅ **Swagger/OpenAPI** - интерактивная документация API
- ✅ **Глобальная обработка ошибок** - стандартизированные ответы

---

## 🏗️ Архитектура

Проект построен на основе **Clean Architecture** (Чистая архитектура) с разделением на 4 слоя:

```
┌─────────────────────────────────────────┐
│   IT-outCRM (Presentation Layer)       │  ← Web API, Controllers, Middleware
├─────────────────────────────────────────┤
│   IT-outCRM.Application                │  ← Business Logic, DTOs, Services
│   (Application Layer)                   │     Validators, Interfaces
├─────────────────────────────────────────┤
│   IT-outCRM.Infrastructure              │  ← Data Access, Repositories
│   (Infrastructure Layer)                │     DbContext, EF Configurations
├─────────────────────────────────────────┤
│   IT-outCRM.Domain                      │  ← Entities, Domain Models
│   (Domain Layer)                        │     Business Rules
└─────────────────────────────────────────┘
```

### Диаграмма зависимостей:

```
IT-outCRM (API) ─────┐
                      ↓
        IT-outCRM.Application ─────┐
                ↓                   ↓
    IT-outCRM.Domain ← IT-outCRM.Infrastructure
```

**Принципы:**
- Domain Layer не зависит ни от чего
- Application Layer зависит только от Domain
- Infrastructure зависит от Domain и Application
- API зависит от всех слоёв

---

## 🛠️ Технологии

### Backend:
- **.NET 10.0** (Preview) - фреймворк для разработки
- **ASP.NET Core** - Web API
- **C# 13** - язык программирования

### База данных:
- **PostgreSQL 17** - основная СУБД
- **Entity Framework Core 9.0** - ORM
- **Npgsql 9.0.4** - драйвер PostgreSQL для .NET

### Безопасность:
- **JWT (JSON Web Tokens)** - аутентификация
- **BCrypt.Net** - хеширование паролей
- **Role-based Authorization** - авторизация по ролям

### Валидация и маппинг:
- **FluentValidation 11.11** - валидация DTO
- **AutoMapper 13.0** - маппинг объектов

### DevOps:
- **Docker & Docker Compose** - контейнеризация
- **pgAdmin 4** - инструмент администрирования БД
- **EF Core Migrations** - версионирование схемы БД

### API:
- **OpenAPI/Swagger** - документация API
- **RESTful** - архитектурный стиль

---

## ⚡ Функциональность

### Управление сущностями:

| Модуль | CRUD | Пагинация | Фильтрация | Поиск |
|--------|------|-----------|------------|-------|
| **Accounts** (Аккаунты) | ✅ | ✅ | По статусу | - |
| **Orders** (Заказы) | ✅ | ✅ | По статусу, клиенту, исполнителю | - |
| **Customers** (Клиенты) | ✅ | ✅ | По аккаунту, компании | - |
| **Companies** (Компании) | ✅ | ✅ | - | По ИНН |
| **Executors** (Исполнители) | ✅ | ✅ | По аккаунту, компании | Топ по заказам |
| **ContactPersons** (Контакты) | ✅ | ✅ | - | По email |

### Аутентификация и авторизация:

- **Регистрация** - создание нового пользователя
- **Вход** - получение JWT токена
- **Профиль** - информация о текущем пользователе
- **Роли**:
  - `Admin` - полный доступ (CRUD + Delete)
  - `Manager` - Create, Read, Update
  - `User` - только Read

### Дополнительные возможности:

- 📊 **Пагинация** - все списки поддерживают пагинацию
- 🔍 **Фильтрация** - по связанным сущностям и статусам
- ✅ **Валидация** - FluentValidation для всех входных данных
- 🛡️ **Глобальная обработка ошибок** - стандартизированные ответы
- 📝 **Логирование** - ILogger для всех операций

---

## 📦 Установка

### Требования:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (или выше)
- [PostgreSQL 17](https://www.postgresql.org/download/) (или через Docker)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (опционально)

### 1. Клонирование репозитория:

```bash
git clone https://github.com/yourusername/IT-outCRM.git
cd IT-outCRM
```

### 2. Настройка базы данных:

#### Вариант A: Docker (рекомендуется):

```bash
cd IT-outCRM.Infrastructure
docker-compose up -d
```

Docker Compose запустит:
- PostgreSQL 17 на порту `5432`
- pgAdmin 4 на порту `5050`

#### Вариант B: Локальная PostgreSQL:

Создайте базу данных:
```sql
CREATE DATABASE crm_db;
```

### 3. Настройка строки подключения:

Отредактируйте `IT-outCRM/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=crm_db;Username=postgres;Password=password123"
  }
}
```

⚠️ **Для продакшена** используйте User Secrets или переменные окружения!

### 4. Применение миграций:

```bash
cd IT-outCRM
dotnet ef database update --project ../IT-outCRM.Infrastructure --startup-project .
```

### 5. Установка зависимостей:

```bash
dotnet restore
```

---

## 🚀 Запуск

### Режим разработки:

```bash
cd IT-outCRM
dotnet run
```

Приложение запустится на:
- HTTP: `http://localhost:5295`
- HTTPS: `https://localhost:7224`

### Режим продакшена:

```bash
dotnet build -c Release
dotnet run -c Release
```

### С указанием конкретной конфигурации:

```bash
dotnet run --launch-profile http
```

---

## 📡 API

### Базовый URL:

```
http://localhost:5295/api
```

### Документация API:

После запуска приложения в режиме разработки, Swagger UI доступен по адресу:

```
http://localhost:5295/swagger
```

### Основные эндпоинты:

#### 🔐 Аутентификация (`/api/auth`):

| Метод | Endpoint | Описание | Авторизация |
|-------|----------|----------|-------------|
| POST | `/auth/register` | Регистрация | - |
| POST | `/auth/login` | Вход | - |
| GET | `/auth/profile` | Профиль | ✅ Bearer Token |
| GET | `/auth/test` | Проверка токена | ✅ Bearer Token |

#### 📊 Аккаунты (`/api/accounts`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/accounts` | Список (пагинация) | User+ |
| GET | `/accounts/{id}` | Получить по ID | User+ |
| GET | `/accounts/status/{statusId}` | Фильтр по статусу | User+ |
| POST | `/accounts` | Создать | Manager+ |
| PUT | `/accounts/{id}` | Обновить | Manager+ |
| DELETE | `/accounts/{id}` | Удалить | Admin |

#### 📦 Заказы (`/api/orders`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/orders` | Список (пагинация) | User+ |
| GET | `/orders/{id}` | Получить по ID | User+ |
| GET | `/orders/customer/{customerId}` | По клиенту | User+ |
| GET | `/orders/executor/{executorId}` | По исполнителю | User+ |
| GET | `/orders/status/{statusId}` | По статусу | User+ |
| POST | `/orders` | Создать | Manager+ |
| PUT | `/orders/{id}` | Обновить | Manager+ |
| PATCH | `/orders/{id}/status` | Изменить статус | Manager+ |
| DELETE | `/orders/{id}` | Удалить | Admin |

#### 👥 Клиенты (`/api/customers`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/customers` | Список (пагинация) | User+ |
| GET | `/customers/{id}` | Получить по ID | User+ |
| GET | `/customers/account/{accountId}` | По аккаунту | User+ |
| GET | `/customers/company/{companyId}` | По компании | User+ |
| POST | `/customers` | Создать | Manager+ |
| PUT | `/customers/{id}` | Обновить | Manager+ |
| DELETE | `/customers/{id}` | Удалить | Admin |

#### 🏢 Компании (`/api/companies`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/companies` | Список (пагинация) | User+ |
| GET | `/companies/{id}` | Получить по ID | User+ |
| GET | `/companies/inn/{inn}` | Поиск по ИНН | User+ |
| POST | `/companies` | Создать | Manager+ |
| PUT | `/companies/{id}` | Обновить | Manager+ |
| DELETE | `/companies/{id}` | Удалить | Admin |

#### 👨‍💼 Исполнители (`/api/executors`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/executors` | Список (пагинация) | User+ |
| GET | `/executors/{id}` | Получить по ID | User+ |
| GET | `/executors/top` | Топ исполнителей | User+ |
| GET | `/executors/account/{accountId}` | По аккаунту | User+ |
| GET | `/executors/company/{companyId}` | По компании | User+ |
| POST | `/executors` | Создать | Manager+ |
| PUT | `/executors/{id}` | Обновить | Manager+ |
| DELETE | `/executors/{id}` | Удалить | Admin |

#### 📞 Контактные лица (`/api/contactpersons`):

| Метод | Endpoint | Описание | Роль |
|-------|----------|----------|------|
| GET | `/contactpersons` | Список (пагинация) | User+ |
| GET | `/contactpersons/{id}` | Получить по ID | User+ |
| GET | `/contactpersons/email/{email}` | Поиск по email | User+ |
| POST | `/contactpersons` | Создать | Manager+ |
| PUT | `/contactpersons/{id}` | Обновить | Manager+ |
| DELETE | `/contactpersons/{id}` | Удалить | Admin |

### Примеры запросов:

#### Регистрация:
```bash
curl -X POST http://localhost:5295/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@example.com",
    "password": "Admin123!",
    "role": "Admin"
  }'
```

#### Вход:
```bash
curl -X POST http://localhost:5295/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

#### Получение списка аккаунтов (с токеном):
```bash
curl -X GET http://localhost:5295/api/accounts?page=1&pageSize=10 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### Создание нового заказа:
```bash
curl -X POST http://localhost:5295/api/orders \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Разработка сайта",
    "description": "Корпоративный сайт",
    "price": 150000.00,
    "customerId": "customer-guid",
    "executorId": "executor-guid",
    "orderStatusId": "status-guid",
    "supportTeamId": "team-guid"
  }'
```

---

## 📂 Структура проекта

```
IT-outCRM/
│
├── IT-outCRM/                          # 🎮 Web API Layer (Presentation)
│   ├── Controllers/                    # API контроллеры (8)
│   │   ├── AuthController.cs
│   │   ├── AccountsController.cs
│   │   ├── OrdersController.cs
│   │   ├── CustomersController.cs
│   │   ├── CompaniesController.cs
│   │   ├── ExecutorsController.cs
│   │   ├── ContactPersonsController.cs
│   │   └── TestController.cs
│   ├── Middleware/                     # Middleware
│   │   ├── GlobalExceptionHandlerMiddleware.cs
│   │   └── ErrorResponse.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json               # Конфигурация (JWT, БД)
│   ├── appsettings.Development.json
│   ├── Program.cs                     # Точка входа
│   └── IT-outCRM.csproj
│
├── IT-outCRM.Application/              # 💼 Application Layer
│   ├── DTOs/                          # Data Transfer Objects (24)
│   │   ├── Account/
│   │   ├── Order/
│   │   ├── Customer/
│   │   ├── Company/
│   │   ├── Executor/
│   │   ├── ContactPerson/
│   │   ├── Auth/
│   │   └── Common/
│   ├── Interfaces/                    # Интерфейсы (16)
│   │   ├── Repositories/              # Repository интерфейсы
│   │   ├── Services/                  # Service интерфейсы
│   │   └── IUnitOfWork.cs
│   ├── Services/                      # Реализация сервисов (8)
│   │   ├── AccountService.cs
│   │   ├── OrderService.cs
│   │   ├── CustomerService.cs
│   │   ├── CompanyService.cs
│   │   ├── ExecutorService.cs
│   │   ├── ContactPersonService.cs
│   │   ├── AuthService.cs
│   │   └── JwtService.cs
│   ├── Validators/                    # FluentValidation (10)
│   │   ├── Account/
│   │   ├── Order/
│   │   ├── Company/
│   │   ├── ContactPerson/
│   │   └── Auth/
│   ├── Mappings/                      # AutoMapper профили (7)
│   │   ├── AccountMappingProfile.cs
│   │   ├── OrderMappingProfile.cs
│   │   ├── CustomerMappingProfile.cs
│   │   ├── CompanyMappingProfile.cs
│   │   ├── ExecutorMappingProfile.cs
│   │   ├── ContactPersonMappingProfile.cs
│   │   └── UserMappingProfile.cs
│   ├── DependencyInjection.cs         # DI регистрация
│   ├── README.md
│   └── IT-outCRM.Application.csproj
│
├── IT-outCRM.Infrastructure/           # 🔧 Infrastructure Layer
│   ├── Repositories/                  # Реализация репозиториев (9)
│   │   ├── GenericRepository.cs
│   │   ├── AccountRepository.cs
│   │   ├── OrderRepository.cs
│   │   ├── CustomerRepository.cs
│   │   ├── CompanyRepository.cs
│   │   ├── ExecutorRepository.cs
│   │   ├── ContactPersonRepository.cs
│   │   ├── UserRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Mappings/                      # EF Core конфигурации (11)
│   │   ├── AccountConfiguration.cs
│   │   ├── OrderConfiguration.cs
│   │   ├── CustomerConfiguration.cs
│   │   ├── CompanyConfiguration.cs
│   │   ├── ExecutorConfiguration.cs
│   │   ├── ContactPersonConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   ├── AccountStatusConfiguration.cs
│   │   ├── OrderStatusConfiguration.cs
│   │   ├── OrderSupportTeamConfiguration.cs
│   │   └── AdminConfiguration.cs
│   ├── Migrations/                    # EF Core миграции (4)
│   │   ├── InitialCreate.cs
│   │   ├── InitialUpdate.cs
│   │   ├── AddUserEntity.cs
│   │   ├── RefactorContactPersonAndAddJsonIgnore.cs
│   │   └── CrmDbContextModelSnapshot.cs
│   ├── CrmDbContext.cs                # DbContext
│   ├── DependencyInjection.cs         # DI регистрация
│   ├── docker-compose.yml             # Docker Compose
│   └── IT-outCRM.Infrastructure.csproj
│
├── IT-outCRM.Domain/                   # 📦 Domain Layer
│   └── Domain/
│       └── Entity/                    # Доменные сущности (11)
│           ├── Account.cs
│           ├── Order.cs
│           ├── Customer.cs
│           ├── Company.cs
│           ├── Executor.cs
│           ├── ContactPerson.cs
│           ├── User.cs
│           ├── AccountStatus.cs
│           ├── OrderStatus.cs
│           ├── OrderSupportTeam.cs
│           └── Admin.cs
│
├── API_AND_AUTH_IMPLEMENTATION.md     # Документация API
├── IMPLEMENTATION_SUMMARY.md          # Итоги реализации
├── LOGIC_FIXES_SUMMARY.md            # Исправления логики
├── TESTING_RESULTS.md                # Результаты тестирования
├── README.md                         # Этот файл
└── IT-outCRM.slnx                    # Solution файл
```

**Статистика:**
- **C# файлов:** ~109
- **Строк кода:** ~4,600
- **API Endpoints:** 52
- **Controllers:** 8
- **Services:** 8
- **Repositories:** 9
- **DTOs:** 24
- **Validators:** 10
- **Migrations:** 4

---

## 🗄️ База данных

### Схема:

```sql
-- Основные таблицы
AccountStatuses       (id, name, description)
Accounts             (id, company_name, founding_date, account_status_id)
Companies            (id, name, inn, legal_form, contact_person_id)
ContactPersons       (id, first_name, last_name, middle_name, email, phone, role)
Customers            (id, account_id, company_id)
Executors            (id, completed_orders, account_id, company_id)
OrderStatuses        (id, name, description)
OrderSupportTeams    (id, name, description)
Orders               (id, name, description, price, customer_id, executor_id, 
                      order_status_id, support_team_id)
Users                (id, username, email, password_hash, role)
Admins               (id, account_id)
```

### Связи:

```
Account 1:N Customer
Account 1:N Executor
Account 1:N Admin
Account N:1 AccountStatus

Company 1:N Customer
Company 1:N Executor
Company 1:1 ContactPerson

Order N:1 Customer
Order N:1 Executor
Order N:1 OrderStatus
Order N:1 OrderSupportTeam
```

### Миграции:

Список применённых миграций:
1. `InitialCreate` - начальная схема
2. `InitialUpdate` - обновления схемы
3. `AddUserEntity` - добавление пользователей
4. `RefactorContactPersonAndAddJsonIgnore` - исправление ContactPerson

**Применение миграций:**
```bash
dotnet ef database update
```

**Откат миграции:**
```bash
dotnet ef database update <MigrationName>
```

**Создание новой миграции:**
```bash
dotnet ef migrations add <MigrationName> --project IT-outCRM.Infrastructure --startup-project IT-outCRM
```

---

## 🔄 Последние обновления

### v1.2.0 (28 октября 2025) - Исправление логики

#### ✅ Исправлено:

**1. ContactPerson - убрано дублирование полей**
- ❌ Удалено поле `Name` (избыточное)
- ✅ Переименовано `SurName` → `MiddleName` (понятнее)
- ✅ Добавлено вычисляемое свойство `FullName`

**2. Циклические зависимости - добавлен [JsonIgnore]**
- ✅ Все навигационные свойства помечены `[JsonIgnore]`
- ✅ Устранены проблемы с сериализацией JSON
- ✅ API возвращает только ID связанных сущностей

**Детали:** См. [LOGIC_FIXES_SUMMARY.md](LOGIC_FIXES_SUMMARY.md)

### v1.1.0 (28 октября 2025) - API и JWT

- ✅ Реализованы 7 REST контроллеров (52 эндпоинта)
- ✅ JWT аутентификация с ролями
- ✅ Валидация всех входных данных
- ✅ Пагинация и фильтрация

**Детали:** См. [API_AND_AUTH_IMPLEMENTATION.md](API_AND_AUTH_IMPLEMENTATION.md)

### v1.0.0 (14 октября 2025) - Первый релиз

- ✅ Clean Architecture
- ✅ Базовая структура проекта
- ✅ EF Core с PostgreSQL
- ✅ Docker Compose

---

## 👨‍💻 Разработка

### Добавление новой сущности:

1. **Domain Layer** - создайте Entity
2. **Application Layer** - создайте DTOs, Validators, Service Interface
3. **Infrastructure Layer** - создайте Repository, EF Configuration
4. **Application Layer** - реализуйте Service
5. **Infrastructure Layer** - реализуйте Repository
6. **API Layer** - создайте Controller
7. Создайте миграцию: `dotnet ef migrations add AddNewEntity`
8. Примените миграцию: `dotnet ef database update`

### Тестирование API:

Используйте Swagger UI или Postman:
- Swagger: `http://localhost:5295/swagger`
- Коллекция Postman: (будет добавлена)

### Сборка Release:

```bash
dotnet build -c Release
```

### Публикация:

```bash
dotnet publish -c Release -o ./publish
```

---

## 🐛 Известные проблемы

⚠️ **Для разработки (не критично):**
1. Пароли в `appsettings.json` - переместить в User Secrets
2. CORS `AllowAnyOrigin` - настроить конкретные домены для продакшена
3. `EnsureCreated` вместо `Migrate` в `Program.cs` - изменить для продакшена
4. Отсутствуют Refresh Tokens - добавить в будущем
5. Нет Rate Limiting - добавить для защиты от brute-force
6. Нет Unit/Integration тестов - создать тестовый проект

**Подробнее:** См. начальный анализ проекта

---

## 📝 Лицензия

Этот проект распространяется под лицензией MIT. См. файл [LICENSE](LICENSE) для подробностей.

---

## 👥 Контакты

- **Автор:** [Ваше имя]
- **Email:** your.email@example.com
- **GitHub:** [@yourusername](https://github.com/yourusername)

---

## 🎉 Благодарности

- Команда .NET за отличный фреймворк
- Сообщество PostgreSQL
- Все контрибьюторы open-source библиотек

---

## 📚 Дополнительная документация

- [Application Layer README](IT-outCRM.Application/README.md)
- [API Implementation Details](API_AND_AUTH_IMPLEMENTATION.md)
- [Implementation Summary](IMPLEMENTATION_SUMMARY.md)
- [Logic Fixes Summary](LOGIC_FIXES_SUMMARY.md)
- [Testing Results](TESTING_RESULTS.md)

---

**Создано с ❤️ для IT-аутсорсинговых компаний**

**Версия:** 1.2.0  
**Дата:** 28 октября 2025  
**Статус:** ✅ Готов к разработке

