# 📚 Repository Pattern Guide

## 🎯 Правильное использование Generic Repository

### ⚠️ Проблема: Leaky Abstraction

**Плохо:** Выставлять EF Core детали через публичный API
```csharp
public interface IRepository<T> 
{
    // ❌ Expression<Func<T, bool>> - это деталь реализации EF Core!
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
```

**Хорошо:** Скрыть детали реализации, предоставить domain-specific методы
```csharp
public interface IUserRepository 
{
    // ✅ Ясный контракт, независимый от ORM
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}
```

---

## ✅ Текущая реализация в IT-outCRM

### 1. Интерфейс без протечек

```csharp
// IT-outCRM.Application/Interfaces/Repositories/IGenericRepository.cs
public interface IGenericRepository<T> : IPagedRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync();
    Task<bool> ExistsAsync(Guid id);
    
    // ❌ FindAsync(Expression) УБРАН из интерфейса!
}
```

### 2. Protected метод для наследников

```csharp
// IT-outCRM.Infrastructure/Repositories/GenericRepository.cs
public class GenericRepository<T> : IGenericRepository<T>
{
    protected readonly DbSet<T> _dbSet;
    
    // ✅ protected - доступен только наследникам
    protected virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
}
```

### 3. Конкретный репозиторий с domain-specific методами

```csharp
// IT-outCRM.Infrastructure/Repositories/UserRepository.cs
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(CrmDbContext context) : base(context) { }
    
    // ✅ Использование protected FindAsync ВНУТРИ для реализации
    public async Task<User?> GetByUsernameAsync(string username)
    {
        // Можем использовать protected метод или напрямую _dbSet
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    // ✅ Можем использовать FindAsync если нужно
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await FindAsync(u => u.IsActive);
    }
    
    // ✅ Ясный, domain-specific контракт
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
```

---

## 📖 Принципы

### ✅ **DO** (Делайте так):

1. **Domain-specific интерфейсы**
   ```csharp
   IUserRepository, IOrderRepository, IAccountRepository
   ```

2. **Ясные контракты методов**
   ```csharp
   Task<User?> GetByUsernameAsync(string username)
   Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
   ```

3. **GenericRepository как вспомогательный класс**
   ```csharp
   public class UserRepository : GenericRepository<User> { ... }
   ```

4. **Protected методы для переиспользования**
   ```csharp
   protected Task<IEnumerable<T>> FindAsync(...)
   ```

---

### ❌ **DON'T** (Не делайте так):

1. **Не выставляйте generic интерфейс публично**
   ```csharp
   ❌ services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
   ✅ services.AddScoped<IUserRepository, UserRepository>();
   ```

2. **Не используйте Expression в публичном API**
   ```csharp
   ❌ Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
   ✅ Task<User?> GetByUsernameAsync(string username)
   ```

3. **Не делайте методы "на всякий случай"**
   ```csharp
   ❌ public Task<IEnumerable<T>> GetAll() // Для Orders с 1M записей?
   ✅ public Task<(IEnumerable<T>, int)> GetPagedAsync(int page, int size)
   ```

---

## 🎯 Пример: Правильная архитектура

### Слой Application (контракты)

```csharp
// Interfaces/Repositories/IUserRepository.cs
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
```

### Слой Infrastructure (реализация)

```csharp
// Repositories/UserRepository.cs
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(CrmDbContext context) : base(context) { }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
```

### Регистрация в DI

```csharp
// DependencyInjection.cs
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();

// ❌ НЕ регистрируйте так:
// services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

---

## 📚 Источники

- [Why the generic repository is just a lazy anti-pattern](https://www.ben-morris.com/why-the-generic-repository-is-just-a-lazy-anti-pattern/)
- [Martin Fowler - Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [EF Core already implements Repository and UoW](https://docs.microsoft.com/en-us/ef/core/)

---

## 🎯 Ключевые выводы

1. **Generic Repository полезен** - как вспомогательный базовый класс
2. **Но не как публичный контракт** - используйте domain-specific интерфейсы
3. **Expression<Func<T, bool>> - это протечка** - скройте детали EF Core
4. **Ясность важнее переиспользования** - каждый метод должен иметь смысл

---

**Версия:** 1.0  
**Дата:** Ноябрь 2025  
**Статус:** ✅ Реализовано в IT-outCRM
