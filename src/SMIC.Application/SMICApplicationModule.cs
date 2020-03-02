using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SMIC.Authorization;
using SMIC.SDIM.Mapper;

namespace SMIC
{
    [DependsOn(
        typeof(SMICCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class SMICApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<SMICAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(SMICApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );

            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                HomeInfoMapper.CreateMappings(configuration);
            });

        }
    }
}
