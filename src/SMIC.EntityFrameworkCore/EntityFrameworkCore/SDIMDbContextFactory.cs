using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SMIC.Configuration;
using SMIC.Web;

namespace SMIC.EntityFrameworkCore
{
    public class SDIMDbContextFactory : IDesignTimeDbContextFactory<SDIMDbContext>
    {
        public SDIMDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SDIMDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            SDIMDbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString(SMICConsts.SDIMConnectionStringName)
            );

            return new SDIMDbContext(builder.Options);
        }
    }
}