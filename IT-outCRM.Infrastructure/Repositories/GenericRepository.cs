using System.Linq.Expressions;
using IT_outCRM.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    /// <summary>
    /// Базовый репозиторий с CRUD операциями и пагинацией
    /// Реализует IGenericRepository, который включает IPagedRepository (ISP)
    /// 
    /// ВАЖНО: FindAsync(Expression) намеренно сделан protected для предотвращения
    /// протечки деталей реализации EF Core через публичный API.
    /// Наследники могут использовать его внутренне для реализации domain-specific методов.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CrmDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(CrmDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Защищенный метод для использования в наследниках.
        /// НЕ выставляется через интерфейс для предотвращения leaky abstraction.
        /// Используйте этот метод внутри конкретных репозиториев для создания
        /// domain-specific методов с ясными контрактами.
        /// </summary>
        protected virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Реализация IPagedRepository - эффективная пагинация на уровне БД
        /// </summary>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();
            var items = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (items, totalCount);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }
    }
}

