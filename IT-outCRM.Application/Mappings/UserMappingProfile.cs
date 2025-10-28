using AutoMapper;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}

