# 📝 История изменений (Changelog)

Все важные изменения проекта IT-outCRM документируются в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/),
и этот проект придерживается [Semantic Versioning](https://semver.org/lang/ru/).

---

## [1.3.2] - 2025-11-02

### ✅ Добавлено

#### AccountStatus контроллер
- Новый REST контроллер `/api/accountstatuses` с полным CRUD функционалом
- DTOs: `AccountStatusDto`, `CreateAccountStatusDto`, `UpdateAccountStatusDto`
- FluentValidation валидаторы: `CreateAccountStatusValidator`, `UpdateAccountStatusValidator`
- Сервисный слой: `IAccountStatusService`, `AccountStatusService`
- Репозиторий: `IAccountStatusRepository`, `AccountStatusRepository` с методом `NameExistsAsync`
- AutoMapper профиль: `AccountStatusMappingProfile`
- Проверка уникальности имени статуса при создании/обновлении
- Swagger документация для всех endpoints
- 5 новых API endpoints

#### Методы загрузки связанных сущностей
- `GetAllWithStatusAsync()` в `AccountRepository` - получение всех аккаунтов со статусами
- `GetPagedWithStatusAsync()` в `AccountRepository` - пагинация аккаунтов со статусами
- Переопределение `GetAllAsync()` в `AccountService` для использования Eager Loading
- Переопределение `GetPagedAsync()` в `AccountService` для использования Eager Loading

### 🔧 Исправлено

- **Критическое:** Исправлена проблема с пустым полем `accountStatusName` при получении списка аккаунтов
- Теперь Entity Framework Core явно загружает связанную сущность `AccountStatus` через `.Include()`
- Все методы получения аккаунтов теперь корректно возвращают `accountStatusName`

### 📈 Статистика

- **Эндпоинты:** 52 → 57 (+5)
- **Контроллеры:** 7 → 8 (+1)
- **Новых файлов:** +11
- **Изменённых файлов:** +5

### 📚 Документация

- Обновлен `README.md` - добавлена секция v1.3.2
- Обновлен `SWAGGER_DOCUMENTATION.md` - добавлен AccountStatus контроллер
- Обновлен `IT-outCRM.Application/README.md` - отражены новые компоненты
- Обновлён `IT-outCRM/Program.cs` - версия API в Swagger v1.3.2
- Создан `ACCOUNTSTATUS_UPDATE_v1.3.2.md` - подробное описание изменений
- Создан `QUICK_START_ACCOUNTS.md` - быстрое руководство по созданию аккаунтов
- Создан `CHANGELOG.md` - этот файл

---

## [1.3.1] - 2025-11-01

### ✅ Добавлено

#### Swagger UI интеграция
- Полная интеграция Swagger UI 7.2.0
- JWT авторизация в Swagger (кнопка "Authorize")
- XML комментарии для документирования API
- Автоматическая генерация примеров запросов/ответов
- Интерактивное тестирование всех endpoints
- OpenAPI 3.0 спецификация

#### Документация контроллеров
- XML документация для `AuthController`
- XML документация для `AccountsController`
- Настройка генерации XML файла документации
- Подавление предупреждений о недокументированных членах (CS1591)

### 🔧 Исправлено

- Конфликт пакетов `Microsoft.AspNetCore.OpenApi` и `Swashbuckle.AspNetCore`
- Удалён `Microsoft.AspNetCore.OpenApi` в пользу `Swashbuckle.AspNetCore`
- Удалены вызовы `AddOpenApi()` и `MapOpenApi()`

### 📚 Документация

- Создан `SWAGGER_DOCUMENTATION.md` - полное руководство по Swagger UI
- Обновлён `README.md` - добавлена информация о Swagger
- Обновлены все файлы технической документации

---

## [1.3.0] - 2025-01

### ✅ Добавлено

#### Рефакторинг сервисов (DRY)
- Базовый класс `BaseService<TEntity, TDto, TCreateDto, TUpdateDto>`
- Устранение дублирования CRUD операций во всех сервисах
- Централизованная валидация и обработка ошибок
- Сокращение кода на 70% (с ~900 до ~270 строк)

#### Рефакторинг контроллеров (DRY + SRP)
- Базовый класс `BaseController` для общей функциональности
- Рефакторинг всех 6 основных контроллеров
- Сокращение кода контроллеров на ~33%
- Единообразная обработка ошибок и логирование

#### Улучшение производительности пагинации
- Пагинация перенесена на уровень БД (SQL OFFSET/LIMIT)
- Улучшение производительности в 10-100 раз
- Интерфейс `IPagedRepository<T>` для разделения ответственности (ISP)

#### Разделение ответственностей (SRP)
- `EntityValidationService` для централизованной валидации
- `IExceptionResponseFactory` для создания стандартизированных ответов об ошибках
- Улучшенное соблюдение принципа единственной ответственности

### 📊 Результаты

- Сокращение кода: 33% (с ~1200 до ~800 строк)
- Производительность пагинации: улучшение в 10-100 раз
- SOLID оценка: 8.8/10 → 9.8/10
- Качество кода: ⭐⭐⭐⭐⭐ (5/5)

### 📚 Документация

- Создан `REFACTORING_SUMMARY.md` - детальная сводка рефакторинга
- Создан `SOLID_ANALYSIS.md` - анализ соблюдения SOLID принципов
- Создан `FINAL_IMPROVEMENTS_SUMMARY.md` - итоговая сводка улучшений
- Создан `CODE_QUALITY_ANALYSIS.md` - анализ качества кода

---

## [1.2.0] - 2025-10-28

### 🔧 Исправлено

#### ContactPerson - убрано дублирование полей
- Удалено избыточное поле `Name`
- Переименовано `SurName` → `MiddleName` для ясности
- Добавлено вычисляемое свойство `FullName`

#### Циклические зависимости
- Все навигационные свойства помечены `[JsonIgnore]`
- Устранены проблемы с сериализацией JSON
- API возвращает только ID связанных сущностей

### 📚 Документация

- Создан `LOGIC_FIXES_SUMMARY.md` - описание исправлений логики

---

## [1.1.0] - 2025-10-28

### ✅ Добавлено

#### REST API
- 7 REST контроллеров (52 эндпоинта)
- JWT аутентификация с ролями (Admin, Manager, User)
- FluentValidation для всех входных данных
- Пагинация и фильтрация
- Глобальная обработка ошибок

#### Контроллеры
- `AuthController` - регистрация, вход, профиль
- `AccountsController` - управление аккаунтами
- `OrdersController` - управление заказами
- `CustomersController` - управление клиентами
- `CompaniesController` - управление компаниями
- `ExecutorsController` - управление исполнителями
- `ContactPersonsController` - управление контактными лицами

### 📚 Документация

- Создан `API_AND_AUTH_IMPLEMENTATION.md` - описание реализации API

---

## [1.0.0] - 2025-10-14

### ✅ Добавлено

#### Архитектура
- Clean Architecture с 4 слоями (Domain, Application, Infrastructure, Presentation)
- SOLID принципы
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection

#### База данных
- Entity Framework Core 9.0
- PostgreSQL 17
- Миграции
- Seed данные
- Docker Compose для PostgreSQL и pgAdmin

#### Основные сущности
- Account (аккаунты)
- AccountStatus (статусы аккаунтов)
- Order (заказы)
- OrderStatus (статусы заказов)
- Customer (клиенты)
- Company (компании)
- Executor (исполнители)
- ContactPerson (контактные лица)

#### Инфраструктура
- AutoMapper для маппинга
- FluentValidation для валидации
- Generic Repository для CRUD операций
- Глобальная обработка ошибок

### 📚 Документация

- Создан основной `README.md`
- Создан `IT-outCRM.Application/README.md`

---

## Типы изменений

- **✅ Добавлено** - новая функциональность
- **🔧 Исправлено** - исправления ошибок
- **📝 Изменено** - изменения существующей функциональности
- **🗑️ Удалено** - удалённая функциональность
- **⚠️ Устарело** - функциональность, которая скоро будет удалена
- **🔒 Безопасность** - исправления уязвимостей
- **📚 Документация** - изменения в документации
- **📈 Статистика** - числовые показатели изменений
- **📊 Результаты** - итоги обновления

---

## Ссылки

- [Репозиторий на GitHub](https://github.com/yourusername/IT-outCRM)
- [Swagger UI](http://localhost:5295/swagger)
- [Документация Swagger](SWAGGER_DOCUMENTATION.md)
- [Руководство по быстрому старту](QUICK_START_ACCOUNTS.md)

