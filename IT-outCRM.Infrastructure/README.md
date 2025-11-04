# IT-outCRM.Infrastructure

## 🐳 Docker Setup с переменными окружения

### Быстрый старт

1. **Создайте `.env` файл:**
   ```bash
   # Windows
   copy .env.example .env
   
   # Linux/macOS
   cp .env.example .env
   ```

2. **Запустите Docker Compose:**
   ```bash
   docker-compose up -d
   ```

**📝 Примечание:** Файл `.env.example` содержит шаблон с заглушками. Для локальной разработки можно использовать простые пароли, для production - сильные уникальные.

---

## 📋 Переменные окружения

### PostgreSQL

| Переменная | Описание | Значение (из docker-compose.yml) |
|-----------|----------|-----------------------------------|
| `POSTGRES_DB` | Имя базы данных | `mydb` |
| `POSTGRES_USER` | Пользователь БД | `postgres` |
| `POSTGRES_PASSWORD` | Пароль БД | `password123` ⚠️ |

### pgAdmin

| Переменная | Описание | Значение (из docker-compose.yml) |
|-----------|----------|-----------------------------------|
| `PGADMIN_EMAIL` | Email для входа | `admin@example.com` |
| `PGADMIN_PASSWORD` | Пароль для входа | `admin123` ⚠️ |

---

## 🔒 Безопасность

### ✅ Что сделано:
- `.env` файл добавлен в `.gitignore`
- Предоставлен `.env.example` с шаблоном переменных окружения

### ⚠️ Важно для локальной разработки:
1. **НЕ КОММИТЬТЕ `.env` файл в Git!**
2. Текущие пароли (`password123`, `admin123`) подходят **ТОЛЬКО для локальной разработки**
3. **Никогда не используйте эти пароли в production!**

### 🔐 Для Production:
1. Используйте **сильные уникальные пароли** (минимум 16 символов)
2. Рекомендуется **Azure Key Vault** или **AWS Secrets Manager**
3. См. [`../SECURITY_GUIDE.md`](../SECURITY_GUIDE.md) для подробностей

---

## 🚀 Команды Docker Compose

### Запуск контейнеров
```bash
docker-compose up -d
```

### Остановка контейнеров
```bash
docker-compose down
```

### Просмотр логов
```bash
# Все контейнеры
docker-compose logs -f

# Только PostgreSQL
docker-compose logs -f postgres

# Только pgAdmin
docker-compose logs -f pgadmin
```

### Проверка статуса
```bash
docker-compose ps
```

### Пересоздание контейнеров
```bash
docker-compose down
docker-compose up -d --force-recreate
```

---

## 🌐 Доступ к сервисам

### PostgreSQL
- **Host:** `localhost`
- **Port:** `5432`
- **Database:** `mydb`
- **User:** `postgres`
- **Password:** `password123`

**Connection String для приложения:**
```
Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password123
```

⚠️ **Для production используйте User Secrets или Azure Key Vault!**

### pgAdmin
- **URL:** http://localhost:8080
- **Email:** `admin@example.com`
- **Password:** `admin123`

**Подключение к PostgreSQL из pgAdmin:**
1. Откройте http://localhost:8080
2. Войдите: `admin@example.com` / `admin123`
3. Add New Server:
   - **Name:** IT-outCRM
   - **Host:** `postgres` (имя сервиса в docker-compose)
   - **Port:** `5432`
   - **Database:** `mydb`
   - **Username:** `postgres`
   - **Password:** `password123`

---

## 📁 Структура

```
IT-outCRM.Infrastructure/
├── docker-compose.yml      # Конфигурация Docker (для локальной разработки)
├── .env.example           # Шаблон переменных окружения (коммитится в Git)
├── .env                   # Локальные переменные (НЕ коммитится!)
├── postgres-data/         # Данные PostgreSQL (автоматически создается)
├── pgadmin-data/          # Данные pgAdmin (автоматически создается)
└── README.md             # Этот файл
```

---

## 🔧 Troubleshooting

### Не создан .env файл

**Решение:**
```bash
# Windows
copy .env.example .env

# Linux/macOS
cp .env.example .env
```

### Порты уже заняты

**Если порт 5432 занят:**
```yaml
ports:
  - "5433:5432"  # Измените первое число
```

**Если порт 8080 занят:**
```yaml
ports:
  - "8081:80"  # Измените первое число
```

### Очистка всех данных

```bash
# Остановка и удаление контейнеров
docker-compose down

# Удаление данных (будут потеряны все данные БД!)
rm -rf postgres-data pgadmin-data  # Linux/macOS
rmdir /s postgres-data pgadmin-data  # Windows

# Запуск заново
docker-compose up -d
```

---

## 📚 Связанная документация

- [Основной README проекта](../README.md)
- [Руководство по безопасности](../SECURITY_GUIDE.md)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

**Версия:** 1.4.0  
**Дата обновления:** Ноябрь 2025  
**Статус:** ✅ Безопасная конфигурация с переменными окружения
