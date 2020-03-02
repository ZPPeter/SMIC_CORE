using System;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SMIC.Configuration;
using SMIC;
namespace SMIC.EntityFrameworkCore
{
    public class MyConnectionStringResolver : DefaultConnectionStringResolver
    {
        private readonly IConfigurationRoot _appConfiguration;

        public MyConnectionStringResolver(IAbpStartupConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment) 
            : base(configuration)
        {
            
            _appConfiguration =
                AppConfigurations.Get(hostingEnvironment.ContentRootPath, hostingEnvironment.EnvironmentName);
        }

        public override string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            if (args["DbContextConcreteType"] as Type == typeof(SDIMDbContext))
            {
                return _appConfiguration.GetConnectionString(SMICConsts.SDIMConnectionStringName);
            }

            return base.GetNameOrConnectionString(args);
        }
    }
}