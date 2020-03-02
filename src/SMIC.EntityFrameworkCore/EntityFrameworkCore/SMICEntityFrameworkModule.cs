using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using SMIC.EntityFrameworkCore.Seed;

using Abp.Dapper;
using Abp.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Generic;

namespace SMIC.EntityFrameworkCore
{
    [DependsOn(
    typeof(SMICCoreModule),
    typeof(AbpZeroCoreEntityFrameworkCoreModule),
    typeof(AbpDapperModule),
    typeof(AbpEntityFrameworkCoreModule)
    )]
    public class SMICEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }
        public bool SkipDbSeedRNMM { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<SMICDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        SMICDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        SMICDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });

                // Configure second DbContext
                Configuration.Modules.AbpEfCore().AddDbContext<SDIMDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        SDIMDbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        SDIMDbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });

            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SMICEntityFrameworkModule).GetAssembly());
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { typeof(SMICEntityFrameworkModule).GetAssembly() });
            //使用mysql必须修改，默认是sqlserver
            //DapperExtensions.DapperExtensions.SqlDialect = new MySqlDialect();

        }

        public override void PostInitialize()
        {
            //上面 public bool SkipDbSeed { get; set; } 定義未賦值所以默認 false
            SkipDbSeed = true;
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
