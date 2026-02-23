using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(CrmDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.Customer).ThenInclude(c => c!.Account)
                .Include(o => o.Executor).ThenInclude(e => e!.Account)
                .Include(o => o.OrderStatus)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }

        public override async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();
            var items = await _dbSet
                .Include(o => o.Customer).ThenInclude(c => c!.Account)
                .Include(o => o.Executor).ThenInclude(e => e!.Account)
                .Include(o => o.OrderStatus)
                .OrderByDescending(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId)
        {
            return await _dbSet
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.Account)
                .Include(o => o.Executor)
                    .ThenInclude(e => e!.Account)
                .Include(o => o.OrderStatus)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByExecutorAsync(Guid executorId)
        {
            return await _dbSet
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.Account)
                .Include(o => o.Executor)
                    .ThenInclude(e => e!.Account)
                .Include(o => o.OrderStatus)
                .Where(o => o.ExecutorId == executorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(Guid statusId)
        {
            return await _dbSet
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.Account)
                .Include(o => o.Executor)
                    .ThenInclude(e => e!.Account)
                .Include(o => o.OrderStatus)
                .Where(o => o.OrderStatusId == statusId)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.Account)
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.Company)
                .Include(o => o.Executor)
                    .ThenInclude(e => e!.Account)
                .Include(o => o.Executor)
                    .ThenInclude(e => e!.Company)
                .Include(o => o.OrderStatus)
                .Include(o => o.SupportTeam)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}

