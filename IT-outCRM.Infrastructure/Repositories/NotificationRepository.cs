using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(CrmDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50)
        {
            return await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}
