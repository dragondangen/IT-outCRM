using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class OrderStatusRepository : GenericRepository<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<OrderStatus?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> NameExistsAsync(string name, Guid excludeId)
        {
            return await _dbSet.AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Id != excludeId);
        }
    }
}

