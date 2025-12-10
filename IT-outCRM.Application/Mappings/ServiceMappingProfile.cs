using AutoMapper;
using IT_outCRM.Application.DTOs.Service;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.ExecutorName,
                    opt => opt.MapFrom(src => src.Executor != null && src.Executor.Account != null 
                        ? src.Executor.Account.CompanyName 
                        : string.Empty));

            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Executor, opt => opt.Ignore());
            
            CreateMap<UpdateServiceDto, Service>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Executor, opt => opt.Ignore());
        }
    }
}

