using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CrmDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAccountRepository Accounts { get; }
        public IAccountStatusRepository AccountStatuses { get; }
        public IOrderRepository Orders { get; }
        public ICustomerRepository Customers { get; }
        public ICompanyRepository Companies { get; }
        public IExecutorRepository Executors { get; }
        public IContactPersonRepository ContactPersons { get; }
        public IUserRepository Users { get; }
        public IOrderStatusRepository OrderStatuses { get; }
        public IServiceRepository Services { get; }
        public IDealRepository Deals { get; }
        public IDealMessageRepository DealMessages { get; }
        public INotificationRepository Notifications { get; }

        public UnitOfWork(CrmDbContext context)
        {
            _context = context;
            
            Accounts = new AccountRepository(_context);
            AccountStatuses = new AccountStatusRepository(_context);
            Orders = new OrderRepository(_context);
            Customers = new CustomerRepository(_context);
            Companies = new CompanyRepository(_context);
            Executors = new ExecutorRepository(_context);
            ContactPersons = new ContactPersonRepository(_context);
            Users = new UserRepository(_context);
            OrderStatuses = new OrderStatusRepository(_context);
            Services = new ServiceRepository(_context);
            Deals = new DealRepository(_context);
            DealMessages = new DealMessageRepository(_context);
            Notifications = new NotificationRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

