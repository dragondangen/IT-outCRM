using IT_outCRM.Application.DTOs.OrderStatus;
using IT_outCRM.Application.DTOs.Common;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IOrderStatusService
    {
        Task<IEnumerable<OrderStatusDto>> GetAllAsync();
        Task<PagedResult<OrderStatusDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<OrderStatusDto?> GetByIdAsync(Guid id);
        Task<OrderStatusDto> CreateAsync(CreateOrderStatusDto createDto);
        Task<OrderStatusDto> UpdateAsync(UpdateOrderStatusDto updateDto);
        Task DeleteAsync(Guid id);
    }
}











