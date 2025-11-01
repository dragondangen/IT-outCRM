using System.Linq.Expressions;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    /// <summary>
    /// Базовый интерфейс репозитория с CRUD операциями и пагинацией
    /// Соблюдение SOLID Interface Segregation Principle:
    /// Пагинация выделена в IPagedRepository, IGenericRepository наследует её
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IGenericRepository<T> : IPagedRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync();
        Task<bool> ExistsAsync(Guid id);
    }
}

