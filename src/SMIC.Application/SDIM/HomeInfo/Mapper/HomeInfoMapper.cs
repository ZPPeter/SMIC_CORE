
using AutoMapper;

using SMIC.SDIM.Dtos;

namespace SMIC.SDIM.Mapper
{

	/// <summary>
    /// 配置HomeInfo的AutoMapper
    /// </summary>
	internal static class HomeInfoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap <HomeInfo,HomeInfoListDto>();
            configuration.CreateMap <HomeInfoListDto,HomeInfo>();

            configuration.CreateMap <HomeInfoDto,HomeInfo>();
            configuration.CreateMap <HomeInfo,HomeInfoDto>();

        }
	}
}
