# 🔐 Настройка секретов для локальной разработки

## ⚠️ КРИТИЧЕСКИ ВАЖНО!

Файл `appsettings.json` **НЕ содержит реальных секретов** - только заглушки!

Для запуска приложения необходимо настроить секреты одним из способов:

---

## 🎯 Вариант 1: User Secrets (рекомендуется для разработки)

### Шаг 1: Инициализируйте User Secrets

```powershell
cd IT-outCRM
dotnet user-secrets init
```

### Шаг 2: Добавьте секреты

```powershell
# Connection String для БД
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password123"

# JWT Secret Key (минимум 64 символа)
dotnet user-secrets set "Jwt:Key" "YourSuperSecretKeyForJWTTokenGenerationShouldBeAtLeast64CharactersLongForMaximumSecurity"
```

### Шаг 3: Проверьте, что секреты добавлены

```powershell
dotnet user-secrets list
```

Вывод должен содержать:
```
ConnectionStrings:DefaultConnection = Host=localhost;...
Jwt:Key = YourSuperSecretKey...
```

---

## 🐳 Вариант 2: Переменные окружения (для Docker)

### Windows PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password123"
$env:Jwt__Key="YourSuperSecretKeyMinimum64Characters"
```

### Linux/macOS:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password123"
export Jwt__Key="YourSuperSecretKeyMinimum64Characters"
```

---

## 🔑 Генерация безопасного JWT ключа

### PowerShell:

```powershell
$bytes = New-Object byte[] 64
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
Write-Host "Ваш JWT ключ: $key"
```

### C# (консоль):

```csharp
var key = new byte[64];
using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
{
    rng.GetBytes(key);
}
Console.WriteLine(Convert.ToBase64String(key));
```

---

## 📝 Настройка Docker PostgreSQL

### Шаг 1: Создайте `.env` файл

```powershell
cd IT-outCRM.Infrastructure
copy .env.example .env
```

### Шаг 2: Отредактируйте `.env`

Откройте `.env` и замените заглушки на реальные пароли:

```env
POSTGRES_DB=mydb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=ваш_сильный_пароль_123!

PGADMIN_EMAIL=admin@example.com
PGADMIN_PASSWORD=другой_сильный_пароль_456!
```

**⚠️ Минимальные требования к паролям:**
- Минимум 12 символов
- Заглавные и строчные буквы
- Цифры
- Специальные символы (!@#$%^&*)

### Шаг 3: Запустите Docker

```powershell
docker-compose up -d
```

---

## ✅ Проверка настройки

### 1. Запустите приложение:

```powershell
cd IT-outCRM
dotnet run
```

### 2. Ожидаемый результат:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5295
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7224
```

**Если видите ошибку "Failed to connect"** - проверьте что:
- Docker контейнеры запущены (`docker-compose ps`)
- User Secrets настроены (`dotnet user-secrets list`)
- Connection String правильный

---

## 🔒 Безопасность

### ✅ Что защищено:

- ✅ `appsettings.json` содержит только заглушки
- ✅ `.env` файл в `.gitignore` (не попадет в Git)
- ✅ User Secrets хранятся локально (вне репозитория)
- ✅ `.env.example` содержит только инструкции

### ❌ НИКОГДА не делайте:

- ❌ Не коммитьте `.env` файл в Git
- ❌ Не храните пароли в `appsettings.json`
- ❌ Не используйте слабые пароли типа `password123` в production
- ❌ Не передавайте секреты через email/чат

---

## 🚀 Production

Для production используйте:
- **Azure Key Vault** (рекомендуется)
- **AWS Secrets Manager**
- **HashiCorp Vault**
- Переменные окружения CI/CD

См. [`SECURITY_GUIDE.md`](SECURITY_GUIDE.md) для подробностей.

---

## 📚 Дополнительные ресурсы

- [ASP.NET Core User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)

---

**Версия:** 1.0  
**Дата:** Ноябрь 2025  
**Статус:** ✅ Готово
