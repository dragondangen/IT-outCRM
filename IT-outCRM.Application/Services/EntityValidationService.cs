using IT_outCRM.Application.Interfaces;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для валидации существования сущностей
    /// Соблюдение SOLID Single Responsibility Principle
    /// Централизует логику проверки существования сущностей
    /// </summary>
    public interface IEntityValidationService
    {
        /// <summary>
        /// Убедиться, что аккаунт существует, иначе выбросить исключение
        /// </summary>
        Task EnsureAccountExistsAsync(Guid id);

        /// <summary>
        /// Убедиться, что клиент существует, иначе выбросить исключение
        /// </summary>
        Task EnsureCustomerExistsAsync(Guid id);

        /// <summary>
        /// Убедиться, что компания существует, иначе выбросить исключение
        /// </summary>
        Task EnsureCompanyExistsAsync(Guid id);

        /// <summary>
        /// Убедиться, что исполнитель существует, иначе выбросить исключение
        /// </summary>
        Task EnsureExecutorExistsAsync(Guid id);

        /// <summary>
        /// Убедиться, что заказ существует, иначе выбросить исключение
        /// </summary>
        Task EnsureOrderExistsAsync(Guid id);
    }

    /// <summary>
    /// Реализация сервиса валидации сущностей
    /// </summary>
    public class EntityValidationService : IEntityValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EntityValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task EnsureAccountExistsAsync(Guid id)
        {
            if (!await _unitOfWork.Accounts.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Account with ID {id} not found");
            }
        }

        public async Task EnsureCustomerExistsAsync(Guid id)
        {
            if (!await _unitOfWork.Customers.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found");
            }
        }

        public async Task EnsureCompanyExistsAsync(Guid id)
        {
            if (!await _unitOfWork.Companies.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Company with ID {id} not found");
            }
        }

        public async Task EnsureExecutorExistsAsync(Guid id)
        {
            if (!await _unitOfWork.Executors.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Executor with ID {id} not found");
            }
        }

        public async Task EnsureOrderExistsAsync(Guid id)
        {
            if (!await _unitOfWork.Orders.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Order with ID {id} not found");
            }
        }
    }
}

