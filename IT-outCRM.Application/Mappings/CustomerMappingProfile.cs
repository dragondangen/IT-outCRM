using AutoMapper;
using IT_outCRM.Application.DTOs.Customer;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.AccountName,
                    opt => opt.MapFrom(src => src.Account != null ? src.Account.CompanyName : string.Empty))
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : string.Empty));

            CreateMap<CreateCustomerDto, Customer>();
            
            CreateMap<UpdateCustomerDto, Customer>();
        }
    }
}

