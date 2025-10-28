using AutoMapper;
using IT_outCRM.Application.DTOs.ContactPerson;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class ContactPersonMappingProfile : Profile
    {
        public ContactPersonMappingProfile()
        {
            CreateMap<ContactPerson, ContactPersonDto>()
                .ForMember(dest => dest.FullName, 
                    opt => opt.MapFrom(src => src.FullName));
            
            CreateMap<CreateContactPersonDto, ContactPerson>();
            
            CreateMap<UpdateContactPersonDto, ContactPerson>();
        }
    }
}

