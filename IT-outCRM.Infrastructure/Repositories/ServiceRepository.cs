using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<Service?> GetServiceWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Executor)
                    .ThenInclude(e => e!.Account)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Service>> GetServicesByExecutorAsync(Guid executorId)
        {
            return await _dbSet
                .Include(s => s.Executor)
                    .ThenInclude(e => e!.Account)
                .Where(s => s.ExecutorId == executorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _dbSet
                .Include(s => s.Executor)
                    .ThenInclude(e => e!.Account)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Переопределяем GetAllAsync для загрузки навигационных свойств
        /// </summary>
        public override async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Executor)
                    .ThenInclude(e => e!.Account)
                .ToListAsync();
        }
    }
}

