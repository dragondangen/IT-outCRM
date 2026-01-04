using AutoMapper;
using IT_outCRM.Application.DTOs.OrderStatus;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class OrderStatusMappingProfile : Profile
    {
        public OrderStatusMappingProfile()
        {
            CreateMap<OrderStatus, OrderStatusDto>();
            CreateMap<CreateOrderStatusDto, OrderStatus>();
            CreateMap<UpdateOrderStatusDto, OrderStatus>();
        }
    }
}











