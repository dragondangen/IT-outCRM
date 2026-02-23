using AutoMapper;
using IT_outCRM.Application.DTOs.Deal;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class DealMappingProfile : Profile
    {
        public DealMappingProfile()
        {
            CreateMap<Deal, DealDto>()
                .ForMember(dest => dest.OrderName,
                    opt => opt.MapFrom(src => src.Order != null ? src.Order.Name : string.Empty))
                .ForMember(dest => dest.OrderStatusName,
                    opt => opt.MapFrom(src => src.Order != null && src.Order.OrderStatus != null
                        ? src.Order.OrderStatus.Name : string.Empty))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null
                        ? src.Customer.Account.CompanyName : string.Empty))
                .ForMember(dest => dest.ExecutorName,
                    opt => opt.MapFrom(src => src.Executor != null && src.Executor.Account != null
                        ? src.Executor.Account.CompanyName : string.Empty))
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.Messages,
                    opt => opt.MapFrom(src => src.Messages));

            CreateMap<CreateDealDto, Deal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Новая"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerRating, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerReview, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutorRating, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutorReview, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Executor, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore());

            CreateMap<UpdateDealDto, Deal>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutorId, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerRating, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerReview, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutorRating, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutorReview, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Executor, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore());

            CreateMap<DealMessage, DealMessageDto>();
            
            CreateMap<CreateDealMessageDto, DealMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenderName, opt => opt.Ignore())
                .ForMember(dest => dest.SenderRole, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Deal, opt => opt.Ignore());
        }
    }
}
