using AutoMapper;
using IT_outCRM.Application.DTOs.AccountStatus;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class AccountStatusMappingProfile : Profile
    {
        public AccountStatusMappingProfile()
        {
            CreateMap<AccountStatus, AccountStatusDto>();
            CreateMap<CreateAccountStatusDto, AccountStatus>();
            CreateMap<UpdateAccountStatusDto, AccountStatus>();
        }
    }
}

