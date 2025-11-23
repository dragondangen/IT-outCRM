# 🚀 IT-outCRM - Система управления взаимоотношениями с клиентами

<div align="center">

![Status](https://img.shields.io/badge/status-active-success.svg)
![Version](https://img.shields.io/badge/version-1.1.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)
![Blazor](https://img.shields.io/badge/Blazor-Server-blueviolet.svg)

</div>

---

## 📋 Содержание

- [О проекте](#о-проекте)
- [Возможности](#возможности)
- [Технологии](#технологии)
- [Быстрый старт](#быстрый-старт)
- [Роли пользователей](#роли-пользователей)
- [Структура проекта](#структура-проекта)
- [Документация](#документация)

---

## 🎯 О проекте

**IT-outCRM** - это современная веб-система для управления заказами, клиентами и взаимоотношениями с ними. Разработана на Blazor Server с использованием лучших практик и современных подходов.

### Основные преимущества:

- 🎨 **Современный UI/UX** - красивый и интуитивно понятный интерфейс
- 🔐 **Безопасность** - JWT-аутентификация и role-based доступ
- ⚡ **Производительность** - быстрая загрузка и отзывчивый интерфейс
- 📱 **Адаптивность** - работает на всех устройствах
- 🎭 **Гибкость ролей** - три уровня доступа для разных типов пользователей

---

## ✨ Возможности

### 🔐 Аутентификация и авторизация
- Регистрация новых пользователей
- Вход в систему с JWT-токеном
- Автоматическое сохранение сессии
- Защита страниц по ролям

### 📊 Dashboard
- Статистика по заказам и клиентам
- Быстрые действия (создание заказа/клиента)
- Последние заказы
- Адаптация под роль пользователя

### 📦 Управление заказами
- **Просмотр** списка всех заказов
- **Создание** новых заказов
- **Редактирование** существующих заказов
- **Удаление** заказов
- **Поиск** по названию и описанию
- **Фильтрация** по статусам
- **Детальный просмотр** с информацией о клиенте и исполнителе

### 👥 Управление клиентами
- **Просмотр** списка клиентов
- **Создание** новых клиентов
- **Редактирование** информации о клиентах
- **Удаление** клиентов
- **Поиск** по аккаунту и компании
- **Просмотр заказов** конкретного клиента

### 👤 Личный кабинет
- Профиль пользователя с информацией о роли
- Страница "Мои заказы" для клиентов
- Статистика по личным заказам

---

## 🛠️ Технологии

### Frontend
- **Blazor Server** (.NET 10) - основной фреймворк
- **Bootstrap 5** - CSS-фреймворк
- **Bootstrap Icons** - иконки
- **C#** - язык программирования

### Backend API
- **ASP.NET Core Web API** (.NET 10)
- **Entity Framework Core** - ORM
- **PostgreSQL** - база данных
- **JWT** - аутентификация

### Архитектура
- **Clean Architecture** - разделение на слои
- **Repository Pattern** - работа с данными
- **Dependency Injection** - управление зависимостями
- **Custom Authentication** - кастомная аутентификация для Blazor

---

## 🚀 Быстрый старт

### Предварительные требования

- **.NET 10 SDK** или выше
- **PostgreSQL** (для backend)
- **Visual Studio 2022** / **Rider** / **VS Code**

### Установка и запуск

1. **Клонируйте репозиторий**
```bash
git clone https://your-repo-url/IT-outCRM.git
cd IT-outCRM
```

2. **Запустите Backend API**
```bash
cd IT-outCRM
dotnet run
# Backend будет доступен на: http://localhost:5295
```

3. **Запустите Frontend (в отдельном терминале)**
```bash
cd IT-outCRM.Blazor
dotnet run
# Frontend будет доступен на: http://localhost:5159
```

4. **Откройте браузер**
```
http://localhost:5159
```

5. **Зарегистрируйтесь или войдите**
- Создайте нового пользователя через `/register`
- Или войдите существующим через `/login`

---

## 🎭 Роли пользователей

### 👑 Администратор (Admin)

**Права доступа:**
- ✅ Полный доступ ко всем функциям
- ✅ Управление всеми заказами
- ✅ Управление всеми клиентами
- ✅ Доступ к отчетам и аналитике
- ✅ Управление пользователями (скоро)

**Навигация:**
```
Dashboard → Все заказы → Клиенты → Отчеты → Профиль
```

---

### 👔 Менеджер (Manager)

**Права доступа:**
- ✅ Просмотр и управление заказами
- ✅ Просмотр и управление клиентами
- ✅ Создание новых заказов и клиентов
- ✅ Редактирование и удаление
- ❌ Нет доступа к административным функциям

**Навигация:**
```
Dashboard → Все заказы → Клиенты → Профиль
```

---

### 👤 Клиент (Client)

**Права доступа:**
- ✅ Просмотр своих заказов
- ✅ Создание новых заказов
- ✅ Просмотр деталей заказов
- ❌ Нет доступа к редактированию
- ❌ Нет доступа к другим клиентам

**Навигация:**
```
Dashboard → Мои заказы → Профиль
```

---

## 📁 Структура проекта

```
IT-outCRM.Blazor/
│
├── Components/              # Blazor компоненты
│   ├── Layout/             # Layout компоненты (NavMenu, MainLayout)
│   ├── Pages/              # Страницы приложения
│   │   ├── Orders/         # Страницы заказов
│   │   │   ├── OrdersList.razor
│   │   │   ├── CreateOrder.razor
│   │   │   ├── EditOrder.razor
│   │   │   └── OrderDetails.razor
│   │   ├── Customers/      # Страницы клиентов
│   │   │   ├── CustomersList.razor
│   │   │   ├── CreateCustomer.razor
│   │   │   ├── EditCustomer.razor
│   │   │   └── CustomerDetails.razor
│   │   ├── Dashboard.razor
│   │   ├── Home.razor
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   ├── Profile.razor
│   │   └── MyOrders.razor
│   └── RedirectToLogin.razor
│
├── Services/               # Сервисы для работы с API
│   ├── Auth/
│   │   ├── IAuthService.cs
│   │   ├── AuthService.cs
│   │   ├── ITokenStorage.cs
│   │   └── TokenStorage.cs
│   ├── Authentication/
│   │   ├── CustomAuthenticationStateProvider.cs
│   │   ├── AuthenticationHttpClientHandler.cs
│   │   └── BlazorAuthenticationHandler.cs
│   ├── IOrderService.cs
│   ├── OrderService.cs
│   ├── ICustomerService.cs
│   ├── CustomerService.cs
│   └── ... (другие сервисы)
│
├── Models/                 # Модели данных
│   ├── AuthResponse.cs
│   ├── LoginModel.cs
│   ├── RegisterModel.cs
│   ├── OrderModel.cs
│   ├── CustomerModel.cs
│   └── ... (другие модели)
│
├── wwwroot/               # Статические файлы
│   ├── css/
│   ├── js/
│   └── favicon.png
│
├── Program.cs             # Точка входа и конфигурация
├── appsettings.json       # Настройки приложения
│
└── Documentation/         # Документация
    ├── README.md          # Этот файл
    ├── CHANGELOG.md       # История изменений
    └── ROLES_GUIDE.md     # Руководство по ролям
```

---

## 📚 Документация

### Основные документы

- 📖 [README.md](README.md) - Основная документация (этот файл)
- 📝 [CHANGELOG.md](CHANGELOG.md) - История изменений
- 🎭 [ROLES_GUIDE.md](ROLES_GUIDE.md) - Подробное руководство по ролям

### Архитектурные решения

#### Аутентификация
```csharp
// JWT токены хранятся в памяти и localStorage
// CustomAuthenticationStateProvider управляет состоянием
// AuthenticationHttpClientHandler автоматически добавляет токены к запросам
```

#### Роли и доступ
```csharp
// Защита страниц
@attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]
@attribute [Authorize(Roles = "Admin,Manager")]

// Условное отображение
@if (context.User.IsInRole("Admin")) { ... }
```

#### Работа с API
```csharp
// Все сервисы используют HttpClient
// Dependency Injection для управления жизненным циклом
// Автоматическая обработка ошибок и логирование
```

---

## 🎨 Скриншоты

### Главная страница
![Landing](https://via.placeholder.com/800x400?text=Landing+Page)

### Dashboard
![Dashboard](https://via.placeholder.com/800x400?text=Dashboard)

### Список заказов
![Orders](https://via.placeholder.com/800x400?text=Orders+List)

### Профиль
![Profile](https://via.placeholder.com/800x400?text=User+Profile)

---

## 🔧 Настройка

### API Base URL

Измените в `appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5295"
  }
}
```

### Таймаут запросов

Настраивается в `Program.cs`:
```csharp
builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30); // Измените здесь
})
```

---

## 🐛 Известные проблемы

1. **Фильтрация заказов для клиентов**
   - В настоящее время клиенты видят все заказы
   - Планируется: фильтрация по CustomerId в следующей версии

2. **Управление пользователями**
   - Функция еще не реализована
   - Планируется: страница управления пользователями для админов

---

## 🚧 Roadmap

### Версия 1.2
- [ ] Управление пользователями (Admin)
- [ ] Изменение паролей
- [ ] Email-уведомления
- [ ] История изменений

### Версия 1.3
- [ ] Отчеты и аналитика
- [ ] Графики (Chart.js)
- [ ] Экспорт в PDF/Excel

### Версия 1.4
- [ ] Прикрепление файлов
- [ ] Комментарии к заказам
- [ ] Real-time уведомления
- [ ] Dark mode

---

## 📞 Поддержка

Если у вас возникли вопросы или проблемы:

1. Проверьте [ROLES_GUIDE.md](ROLES_GUIDE.md) для информации о ролях
2. Посмотрите [CHANGELOG.md](CHANGELOG.md) для истории изменений
3. Откройте issue в GitHub

---

## 📄 Лицензия

Этот проект разработан для учебных/коммерческих целей.

---

## 🙏 Благодарности

- **Blazor Team** - за отличный фреймворк
- **Bootstrap** - за UI компоненты
- **Community** - за поддержку и вдохновение

---

<div align="center">

**Сделано с ❤️ для эффективного управления CRM**

[⬆ Вернуться к началу](#-it-outcrm---система-управления-взаимоотношениями-с-клиентами)

</div>
