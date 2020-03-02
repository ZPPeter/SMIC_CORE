using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SMIC.EntityFrameworkCore
{
    public static class SMICDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SMICDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<SMICDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
