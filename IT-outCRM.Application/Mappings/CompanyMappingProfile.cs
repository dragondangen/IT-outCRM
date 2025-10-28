using AutoMapper;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.ContactPersonId,
                    opt => opt.MapFrom(src => src.ContactPersonID))
                .ForMember(dest => dest.ContactPersonName,
                    opt => opt.MapFrom(src => src.ContactPerson != null 
                        ? $"{src.ContactPerson.FirstName} {src.ContactPerson.LastName}" 
                        : string.Empty));

            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.ContactPersonID, opt => opt.MapFrom(src => src.ContactPersonId));
            
            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.ContactPersonID, opt => opt.MapFrom(src => src.ContactPersonId));
        }
    }
}

