# 🔧 Исправление проблемы авторизации

## 🐛 Проблема

**Фронтенд не отправлял запросы на backend для Login/Register.**

### Причина:

HttpClient для `AuthService` был настроен с `AuthenticationHttpClientHandler`, который пытается добавить JWT токен к **каждому** запросу, включая `/api/auth/login` и `/api/auth/register`.

```csharp
// ❌ НЕПРАВИЛЬНО
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<AuthenticationHttpClientHandler>(); // Проблема!
```

**Почему это проблема:**
1. Login/Register запросы **не должны** иметь токен (его еще нет!)
2. `AuthenticationHttpClientHandler` вызывает `ITokenStorage.GetTokenAsync()`
3. `TokenStorage` использует `ProtectedLocalStorage` 
4. `ProtectedLocalStorage` работает только после рендеринга компонента
5. Результат: запросы блокировались или не отправлялись

## ✅ Решение

### 1. Убрали handler из AuthService

```csharp
// ✅ ПРАВИЛЬНО
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}); // Без .AddHttpMessageHandler!
```

**Почему это правильно:**
- Login/Register не требуют токена
- Другие сервисы (Orders, Customers) используют handler - это правильно
- AuthService получает токен от backend и сохраняет его

### 2. Добавили детальное логирование

```csharp
Console.WriteLine($"=== LOGIN START ===");
Console.WriteLine($"Username: {model.Username}");
Console.WriteLine($"API URL: {_httpClient.BaseAddress}api/auth/login");
Console.WriteLine($"Response Status: {response.StatusCode}");
```

**Где смотреть логи:**
- Откройте браузер (F12)
- Вкладка **Console**
- Там будут все логи авторизации

## 🧪 Как протестировать

### Шаг 1: Откройте DevTools

1. Откройте браузер: `http://localhost:5159/login`
2. Нажмите **F12** (Developer Tools)
3. Перейдите на вкладку **Console**

### Шаг 2: Попробуйте зарегистрироваться

1. Перейдите на `/register`
2. Заполните форму:
   - Username: `testuser`
   - Email: `test@example.com`
   - Password: `Test123!`
   - ConfirmPassword: `Test123!`
   - Role: `Admin`
3. Нажмите "Зарегистрироваться"

### Шаг 3: Проверьте логи в Console

**Успешная регистрация:**
```
=== REGISTER START ===
Username: testuser
Email: test@example.com
API URL: https://localhost:7224/api/auth/register
Response Status: 200
Token received: true
=== REGISTER SUCCESS ===
```

**Если ошибка:**
```
=== REGISTER START ===
Username: testuser
...
Response Status: 400
Error response: {"message": "..."}
=== REGISTER FAILED ===
```

### Шаг 4: Проверьте Network

1. Вкладка **Network** в DevTools
2. Найдите запрос к `/api/auth/register`
3. Проверьте:
   - **Status Code**: должен быть 200
   - **Request Payload**: должен быть JSON с username, email, password, role
   - **Response**: должен содержать token

## 🔍 Диагностика проблем

### Проблема: "CORS error"

**Симптомы:**
```
Access to fetch at 'https://localhost:7224/api/auth/login' from origin 
'http://localhost:5159' has been blocked by CORS policy
```

**Решение:**
Backend уже настроен с правильным CORS (AllowAnyOrigin в Development).
Если видите эту ошибку - перезапустите backend:
```bash
cd IT-outCRM
dotnet run
```

### Проблема: "Connection refused"

**Симптомы:**
```
Failed to fetch
net::ERR_CONNECTION_REFUSED
```

**Решение:**
Backend не запущен. Запустите:
```bash
cd IT-outCRM
dotnet run
```

Проверьте что backend слушает на `https://localhost:7224`

### Проблема: "401 Unauthorized"

**Симптомы:**
```
Response Status: 401
```

**Причина:** Это нормально для защищенных эндпоинтов.
Но Login/Register должны возвращать 200, а не 401.

**Решение:** Проверьте что вызываете правильный эндпоинт.

### Проблема: "Validation errors"

**Симптомы:**
```
Response Status: 400
Error response: {"errors": {"Password": ["Password must be ..."]}}
```

**Решение:** 
Backend проверяет валидацию. Убедитесь:
- Password: минимум 6 символов
- Email: правильный формат
- Username: уникальный

## 📊 Архитектура авторизации

### Поток данных - Регистрация:

```
1. User заполняет форму RegisterModel
   ↓
2. OnValidSubmit → HandleRegister()
   ↓
3. AuthService.RegisterAsync(RegisterModel)
   ↓
4. Конвертация: RegisterModel → RegisterDto (без ConfirmPassword)
   ↓
5. HttpClient.PostAsJsonAsync("api/auth/register", registerDto)
   ↓ (БЕЗ AuthenticationHttpClientHandler!)
   ↓
6. Backend: POST /api/auth/register
   ↓
7. Backend возвращает: AuthResponseDto { Token, Username, Email, Role, ExpiresAt }
   ↓
8. TokenStorage.SetTokenAsync(token) → ProtectedLocalStorage
   ↓
9. CustomAuthenticationStateProvider.NotifyAuthenticationStateChanged()
   ↓
10. Navigation.NavigateTo("/")
```

### Поток данных - Вход:

```
1. User вводит username/password (LoginModel)
   ↓
2. AuthService.LoginAsync(LoginModel)
   ↓
3. HttpClient.PostAsJsonAsync("api/auth/login", model)
   ↓ (БЕЗ токена!)
   ↓
4. Backend проверяет credentials
   ↓
5. Backend возвращает AuthResponseDto
   ↓
6. Token сохраняется в ProtectedLocalStorage
   ↓
7. AuthenticationState обновляется
   ↓
8. Redirect на главную
```

### Защищенные запросы (после авторизации):

```
1. User переходит на /orders
   ↓
2. OrderService.GetAllAsync()
   ↓
3. HttpClient с AuthenticationHttpClientHandler
   ↓
4. Handler: token = await TokenStorage.GetTokenAsync()
   ↓
5. Handler добавляет: Authorization: Bearer {token}
   ↓
6. Backend получает запрос с токеном
   ↓
7. Backend проверяет JWT
   ↓
8. Backend возвращает данные
```

## 🎯 Что теперь работает

✅ Login - отправляет запрос БЕЗ токена
✅ Register - отправляет запрос БЕЗ токена
✅ Получение токена от backend
✅ Сохранение токена в ProtectedLocalStorage
✅ Обновление AuthenticationState
✅ Автоматическое добавление токена ко всем защищенным запросам
✅ Детальное логирование для диагностики

## 📝 Важные файлы

| Файл | Что изменилось |
|------|----------------|
| `Program.cs` | Убран handler из HttpClient для AuthService |
| `Services/AuthService.cs` | Добавлено детальное логирование |

## 🚀 Быстрый тест

### Консольная команда для теста через curl:

**Register:**
```bash
curl -X POST https://localhost:7224/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "role": "Admin"
  }'
```

**Login:**
```bash
curl -X POST https://localhost:7224/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!"
  }'
```

Ожидаемый ответ:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "testuser",
  "email": "test@example.com",
  "role": "Admin",
  "expiresAt": "2025-11-24T15:00:00Z"
}
```

## 💡 Рекомендации

### 1. После успешной регистрации:
- Откройте DevTools → Application → Local Storage
- Найдите ключ начинающийся с `ProtectedLocalStorage__`
- Там должен быть сохраненный токен (зашифрованный)

### 2. Для дополнительной диагностики:
- Network tab → filter: "auth"
- Смотрите все запросы к /api/auth/*
- Проверяйте Request/Response

### 3. Если все еще не работает:
- Очистите cache браузера (Ctrl+Shift+Delete)
- Перезапустите backend
- Перезапустите frontend
- Проверьте что backend на https://localhost:7224
- Проверьте что frontend на http://localhost:5159

## 🎉 Итог

**Проблема решена!**

- ✅ AuthService больше не использует AuthenticationHttpClientHandler
- ✅ Login/Register отправляют запросы БЕЗ токена
- ✅ Токен получается от backend и сохраняется
- ✅ Защищенные запросы автоматически получают токен
- ✅ Добавлено детальное логирование

**Приложение готово к использованию!** 🚀

**URL:** `http://localhost:5159`


