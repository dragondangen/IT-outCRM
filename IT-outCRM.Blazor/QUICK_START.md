# 🚀 Быстрый старт IT-outCRM Blazor

## Шаг 1: Запуск Backend API

Перед запуском фронтенда убедитесь, что backend API запущен:

```bash
cd IT-outCRM
dotnet run
```

Backend должен быть доступен по адресу: `https://localhost:5001`

## Шаг 2: Настройка подключения к API

Проверьте настройки в `appsettings.Development.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}
```

Если ваш API запущен на другом порту, измените URL соответственно.

## Шаг 3: Запуск Blazor приложения

```bash
cd IT-outCRM.Blazor
dotnet run
```

Приложение будет доступно по адресу: `https://localhost:5002` (или другой порт из launchSettings.json)

## Шаг 4: Первый вход

1. Откройте браузер и перейдите на `https://localhost:5002`
2. Перейдите на страницу регистрации: `/register`
3. Зарегистрируйте нового пользователя:
   - Username: `admin`
   - Email: `admin@example.com`
   - Password: `Admin123!`
   - Role: `Admin`
4. Вы автоматически войдете в систему

## Шаг 5: Подготовка тестовых данных

### Создание компании и аккаунта через API

Используйте Swagger UI backend API (`https://localhost:5001/swagger`) или HTTP клиент:

**Создать компанию:**
```bash
POST https://localhost:5001/api/companies
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "name": "Тестовая компания",
  "inn": "1234567890",
  "address": "г. Москва, ул. Тестовая, д. 1"
}
```

**Создать аккаунт:**
```bash
POST https://localhost:5001/api/accounts
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "name": "Иванов Иван",
  "email": "ivanov@test.com",
  "phone": "+79001234567",
  "accountStatusId": "YOUR_STATUS_GUID"
}
```

**Получить статус аккаунта:**
```bash
GET https://localhost:5001/api/accountstatuses
```

### Создание исполнителя через API

```bash
POST https://localhost:5001/api/executors
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "accountId": "YOUR_ACCOUNT_GUID",
  "specialization": "Backend Developer"
}
```

### Создание статуса заказа через API

```bash
POST https://localhost:5001/api/accountstatuses
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "name": "Новый",
  "description": "Новый заказ"
}
```

## Шаг 6: Работа с фронтендом

### Создание клиента

1. Перейдите в раздел "Клиенты"
2. Нажмите "Добавить клиента"
3. Вставьте GUID аккаунта и компании из предыдущего шага
4. Сохраните

### Создание заказа

1. Перейдите в раздел "Заказы"
2. Нажмите "Создать заказ"
3. Заполните форму:
   - Название: "Разработка сайта"
   - Описание: "Корпоративный сайт"
   - Цена: 100000
   - Выберите клиента из списка
   - Вставьте GUID исполнителя и статуса
4. Сохраните

## 🎉 Готово!

Теперь вы можете:

- Просматривать дашборд с статистикой
- Управлять заказами
- Управлять клиентами
- Просматривать детали и связи между сущностями

## 💡 Полезные советы

### Получение GUID через Swagger

1. Откройте Swagger UI: `https://localhost:5001/swagger`
2. Авторизуйтесь (нажмите "Authorize", вставьте JWT токен)
3. Используйте GET эндпоинты для получения списков:
   - `/api/accounts` - список аккаунтов
   - `/api/companies` - список компаний
   - `/api/executors` - список исполнителей
   - `/api/accountstatuses` - список статусов
4. Скопируйте нужные GUID

### Структура JWT токена

После входа/регистрации токен автоматически сохраняется в ProtectedLocalStorage.
Для использования в Swagger:
1. Войдите в систему через Blazor
2. Откройте DevTools браузера (F12)
3. Перейдите в Application → Local Storage
4. Найдите ключ `authToken`
5. Скопируйте значение токена

### Порты по умолчанию

- Backend API: `https://localhost:5001`
- Blazor Frontend: `https://localhost:5002`
- Swagger UI: `https://localhost:5001/swagger`

## 🐛 Решение проблем

### CORS ошибки

Если видите ошибки CORS в консоли браузера, убедитесь, что в backend API настроен CORS для вашего фронтенда.

### Ошибки подключения к API

1. Проверьте, что backend запущен
2. Проверьте URL в `appsettings.json`
3. Проверьте сертификаты SSL (для разработки можно доверять dev сертификату)

### Ошибки авторизации

1. Убедитесь, что токен не истек
2. Проверьте роль пользователя (некоторые операции требуют Admin или Manager)
3. Перелогиньтесь, если токен устарел

## 📚 Дополнительная информация

- Полная документация: [README.md](README.md)
- Backend API: [../IT-outCRM/README.md](../IT-outCRM/README.md)
- Swagger документация: `https://localhost:5001/swagger`

