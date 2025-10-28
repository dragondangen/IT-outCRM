using AutoMapper;
using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Mappings
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.AccountStatusName, 
                    opt => opt.MapFrom(src => src.AccountStatus != null ? src.AccountStatus.Name : string.Empty));

            CreateMap<CreateAccountDto, Account>();
            
            CreateMap<UpdateAccountDto, Account>();
        }
    }
}

