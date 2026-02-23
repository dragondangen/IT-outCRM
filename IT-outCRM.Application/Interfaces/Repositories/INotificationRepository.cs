using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
