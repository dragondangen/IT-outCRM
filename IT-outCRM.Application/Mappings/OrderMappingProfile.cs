using AutoMapper;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null 
                        ? src.Customer.Account.CompanyName 
                        : string.Empty))
                .ForMember(dest => dest.ExecutorName,
                    opt => opt.MapFrom(src => src.Executor != null && src.Executor.Account != null 
                        ? src.Executor.Account.CompanyName 
                        : string.Empty))
                .ForMember(dest => dest.OrderStatusName,
                    opt => opt.MapFrom(src => src.OrderStatus != null ? src.OrderStatus.Name : string.Empty));

            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.CustomerId, 
                    opt => opt.MapFrom(src => src.CustomerId.HasValue ? src.CustomerId.Value : Guid.Empty))
                .ForMember(dest => dest.OrderStatusId, 
                    opt => opt.MapFrom(src => src.OrderStatusId.HasValue ? src.OrderStatusId.Value : Guid.Empty))
                .ForMember(dest => dest.SupportTeamId, 
                    opt => opt.MapFrom(src => src.SupportTeamId));
            
            CreateMap<UpdateOrderDto, Order>();
        }
    }
}

