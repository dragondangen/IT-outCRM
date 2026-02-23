using AutoMapper;
using IT_outCRM.Application.DTOs.Deal;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class DealService : BaseService<Deal, DealDto, CreateDealDto, UpdateDealDto>, IDealService
    {
        private readonly IDealRepository _dealRepository;
        private readonly IDealMessageRepository _dealMessageRepository;

        private static readonly HashSet<string> ValidStatuses = new()
        {
            "Новая", "Предложена", "Согласована", "В работе", "На проверке", "Завершена", "Отменена"
        };

        private static readonly Dictionary<string, HashSet<string>> AllowedTransitions = new()
        {
            ["Новая"] = new() { "Предложена", "Отменена" },
            ["Предложена"] = new() { "Согласована", "Отменена" },
            ["Согласована"] = new() { "В работе", "Отменена" },
            ["В работе"] = new() { "На проверке", "Отменена" },
            ["На проверке"] = new() { "Завершена", "В работе", "Отменена" },
            ["Завершена"] = new(),
            ["Отменена"] = new()
        };

        public DealService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _dealRepository = unitOfWork.Deals;
            _dealMessageRepository = unitOfWork.DealMessages;
        }

        protected override IGenericRepository<Deal> Repository => _dealRepository;

        public override async Task<DealDto?> GetByIdAsync(Guid id)
        {
            var deal = await _dealRepository.GetDealWithDetailsAsync(id);
            return deal != null ? _mapper.Map<DealDto>(deal) : null;
        }

        protected override Task<DealDto?> GetByIdAfterCreateAsync(Deal entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<DealDto?>(null);
        }

        protected override Task<DealDto?> GetByIdAfterUpdateAsync(Deal entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<DealDto?>(null);
        }

        public async Task<IEnumerable<DealDto>> GetDealsByCustomerAsync(Guid customerId)
        {
            var deals = await _dealRepository.GetDealsByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<DealDto>>(deals);
        }

        public async Task<IEnumerable<DealDto>> GetDealsByExecutorAsync(Guid executorId)
        {
            var deals = await _dealRepository.GetDealsByExecutorAsync(executorId);
            return _mapper.Map<IEnumerable<DealDto>>(deals);
        }

        public async Task<IEnumerable<DealDto>> GetDealsByOrderAsync(Guid orderId)
        {
            var deals = await _dealRepository.GetDealsByOrderAsync(orderId);
            return _mapper.Map<IEnumerable<DealDto>>(deals);
        }

        public async Task<IEnumerable<DealDto>> GetDealsByStatusAsync(string status)
        {
            var deals = await _dealRepository.GetDealsByStatusAsync(status);
            return _mapper.Map<IEnumerable<DealDto>>(deals);
        }

        public async Task<DealDto?> GetDealWithDetailsAsync(Guid id)
        {
            var deal = await _dealRepository.GetDealWithDetailsAsync(id);
            return deal != null ? _mapper.Map<DealDto>(deal) : null;
        }

        public async Task<DealDto> ChangeStatusAsync(Guid dealId, string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new ArgumentException($"Недопустимый статус: {newStatus}");

            var deal = await _dealRepository.GetByIdAsync(dealId)
                ?? throw new KeyNotFoundException($"Сделка {dealId} не найдена");

            if (!AllowedTransitions.TryGetValue(deal.Status, out var allowed) || !allowed.Contains(newStatus))
                throw new InvalidOperationException(
                    $"Невозможен переход из статуса \"{deal.Status}\" в \"{newStatus}\"");

            deal.Status = newStatus;
            deal.UpdatedAt = DateTime.UtcNow;

            if (newStatus == "В работе")
            {
                var orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("В работе");
                if (orderStatus != null)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(deal.OrderId);
                    if (order != null)
                    {
                        order.OrderStatusId = orderStatus.Id;
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }
            }
            else if (newStatus == "На проверке")
            {
                var orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("На проверке");
                if (orderStatus != null)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(deal.OrderId);
                    if (order != null)
                    {
                        order.OrderStatusId = orderStatus.Id;
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }
            }
            else if (newStatus == "Завершена")
            {
                var orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Завершён");
                if (orderStatus == null)
                    orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Завершен");
                if (orderStatus != null)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(deal.OrderId);
                    if (order != null)
                    {
                        order.OrderStatusId = orderStatus.Id;
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }

                var executor = await _unitOfWork.Executors.GetByIdAsync(deal.ExecutorId);
                if (executor != null)
                {
                    executor.CompletedOrders++;
                    await _unitOfWork.Executors.UpdateAsync(executor);
                }
            }
            else if (newStatus == "Отменена")
            {
                var orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Отменён");
                if (orderStatus == null)
                    orderStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Отменен");
                if (orderStatus != null)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(deal.OrderId);
                    if (order != null)
                    {
                        order.OrderStatusId = orderStatus.Id;
                        await _unitOfWork.Orders.UpdateAsync(order);
                    }
                }
            }

            await _dealRepository.UpdateAsync(deal);
            await NotifyDealStatusChangeAsync(deal, newStatus);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(dealId)
                ?? throw new InvalidOperationException("Не удалось получить обновлённую сделку");
        }

        private async Task NotifyDealStatusChangeAsync(Deal deal, string newStatus)
        {
            var link = $"/deals/{deal.Id}";
            var shortId = deal.Id.ToString()[..8];

            var customerUser = await FindUserByCustomerIdAsync(deal.CustomerId);
            var executorUser = await FindUserByExecutorIdAsync(deal.ExecutorId);

            if (customerUser != null)
            {
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = customerUser.Id,
                    Title = $"Сделка #{shortId}: {newStatus}",
                    Message = $"Статус вашей сделки изменён на \"{newStatus}\"",
                    Type = newStatus == "Завершена" ? "success" : newStatus == "Отменена" ? "error" : "info",
                    Link = link
                });
            }

            if (executorUser != null)
            {
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = executorUser.Id,
                    Title = $"Сделка #{shortId}: {newStatus}",
                    Message = $"Статус сделки изменён на \"{newStatus}\"",
                    Type = newStatus == "Завершена" ? "success" : newStatus == "Отменена" ? "error" : "info",
                    Link = link
                });
            }
        }

        private async Task<User?> FindUserByCustomerIdAsync(Guid customerId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null) return null;
            var company = await _unitOfWork.Companies.GetByIdAsync(customer.CompanyId);
            if (company == null) return null;
            var contact = await _unitOfWork.ContactPersons.GetByIdAsync(company.ContactPersonID);
            if (contact == null) return null;
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.FirstOrDefault(u => u.Email == contact.Email);
        }

        private async Task<User?> FindUserByExecutorIdAsync(Guid executorId)
        {
            var executor = await _unitOfWork.Executors.GetByIdAsync(executorId);
            if (executor == null) return null;
            var company = await _unitOfWork.Companies.GetByIdAsync(executor.CompanyId);
            if (company == null) return null;
            var contact = await _unitOfWork.ContactPersons.GetByIdAsync(company.ContactPersonID);
            if (contact == null) return null;
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.FirstOrDefault(u => u.Email == contact.Email);
        }

        public async Task<DealDto> RateByCustomerAsync(Guid dealId, int rating, string? review)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Оценка должна быть от 1 до 5");

            var deal = await _dealRepository.GetByIdAsync(dealId)
                ?? throw new KeyNotFoundException($"Сделка {dealId} не найдена");

            if (deal.Status != "Завершена")
                throw new InvalidOperationException("Оценить можно только завершённую сделку");

            deal.CustomerRating = rating;
            deal.CustomerReview = review;
            deal.UpdatedAt = DateTime.UtcNow;

            await _dealRepository.UpdateAsync(deal);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(dealId)
                ?? throw new InvalidOperationException("Не удалось получить обновлённую сделку");
        }

        public async Task<DealDto> RateByExecutorAsync(Guid dealId, int rating, string? review)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Оценка должна быть от 1 до 5");

            var deal = await _dealRepository.GetByIdAsync(dealId)
                ?? throw new KeyNotFoundException($"Сделка {dealId} не найдена");

            if (deal.Status != "Завершена")
                throw new InvalidOperationException("Оценить можно только завершённую сделку");

            deal.ExecutorRating = rating;
            deal.ExecutorReview = review;
            deal.UpdatedAt = DateTime.UtcNow;

            await _dealRepository.UpdateAsync(deal);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(dealId)
                ?? throw new InvalidOperationException("Не удалось получить обновлённую сделку");
        }

        public async Task<DealMessageDto> AddMessageAsync(Guid dealId, string senderName, string senderRole, string text)
        {
            var deal = await _dealRepository.GetByIdAsync(dealId)
                ?? throw new KeyNotFoundException($"Сделка {dealId} не найдена");

            if (deal.Status == "Завершена" || deal.Status == "Отменена")
                throw new InvalidOperationException("Нельзя отправлять сообщения в завершённую или отменённую сделку");

            var message = new DealMessage
            {
                Id = Guid.NewGuid(),
                DealId = dealId,
                SenderName = senderName,
                SenderRole = senderRole,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            await _dealMessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DealMessageDto>(message);
        }

        public async Task<IEnumerable<DealMessageDto>> GetMessagesAsync(Guid dealId)
        {
            var messages = await _dealMessageRepository.GetMessagesByDealAsync(dealId);
            return _mapper.Map<IEnumerable<DealMessageDto>>(messages);
        }
    }
}
