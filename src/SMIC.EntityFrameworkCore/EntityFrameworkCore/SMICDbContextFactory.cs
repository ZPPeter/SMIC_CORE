using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SMIC.Configuration;
using SMIC.Web;

namespace SMIC.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class SMICDbContextFactory : IDesignTimeDbContextFactory<SMICDbContext>
    {
        public SMICDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SMICDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            SMICDbContextConfigurer.Configure(builder, configuration.GetConnectionString(SMICConsts.ConnectionStringName));

            return new SMICDbContext(builder.Options);
        }
    }
}
