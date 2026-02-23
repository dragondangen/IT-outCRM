using IT_outCRM.Application.Interfaces.Repositories;

namespace IT_outCRM.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        IAccountStatusRepository AccountStatuses { get; }
        IOrderRepository Orders { get; }
        ICustomerRepository Customers { get; }
        ICompanyRepository Companies { get; }
        IExecutorRepository Executors { get; }
        IContactPersonRepository ContactPersons { get; }
        IUserRepository Users { get; }
        IOrderStatusRepository OrderStatuses { get; }
        IServiceRepository Services { get; }
        IDealRepository Deals { get; }
        IDealMessageRepository DealMessages { get; }
        INotificationRepository Notifications { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

