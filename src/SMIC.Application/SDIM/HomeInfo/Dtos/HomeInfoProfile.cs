using AutoMapper;
namespace SMIC.SDIM.Dtos
{
    public class HomeInfoMapProfile : Profile
    {
        public HomeInfoMapProfile()
        {
            CreateMap<HomeInfoDto, HomeInfo>();
            //CreateMap<HomeInfoDto, HomeInfo>()
            //    .ForMember(x => x.CreationTime, opt => opt.Ignore());

            //CreateMap<CreateUserDto, User>();
            //CreateMap<CreateUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }
}
