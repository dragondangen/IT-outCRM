using AutoMapper;
using IT_outCRM.Application.DTOs.Executor;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class ExecutorMappingProfile : Profile
    {
        public ExecutorMappingProfile()
        {
            CreateMap<Executor, ExecutorDto>()
                .ForMember(dest => dest.AccountName,
                    opt => opt.MapFrom(src => src.Account != null ? src.Account.CompanyName : string.Empty))
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : string.Empty));

            CreateMap<CreateExecutorDto, Executor>();
            
            CreateMap<UpdateExecutorDto, Executor>();
        }
    }
}

