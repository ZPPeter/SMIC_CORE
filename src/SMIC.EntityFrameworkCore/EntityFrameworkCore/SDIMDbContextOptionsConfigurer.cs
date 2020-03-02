using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SMIC.EntityFrameworkCore
{
    public static class SDIMDbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<SDIMDbContext> dbContextOptions,
            string connectionString
        )
        {
            /* This is the single point to configure DbContextOptions for MultipleDbContextEfCoreDemoDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }

        public static void Configure(
            DbContextOptionsBuilder<SDIMDbContext> dbContextOptions,
            DbConnection connection
        )
        {
            /* This is the single point to configure DbContextOptions for MultipleDbContextEfCoreDemoDbContext */
            dbContextOptions.UseSqlServer(connection);
        }
    }
}