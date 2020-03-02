using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using SMIC.Authorization.Roles;
using SMIC.Authorization.Users;
using SMIC.MultiTenancy;
using System.Reflection;

using SMIC.SDIM;
using SMIC.EntityMapper.HomeInfos;

namespace SMIC.EntityFrameworkCore
{
    public class SMICDbContext : AbpZeroDbContext<Tenant, Role, User, SMICDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<HomeInfo> HomeInfos { get; set; }

        public SMICDbContext(DbContextOptions<SMICDbContext> options)
            : base(options)
        {
            // this.Database.SetCommandTimeout(0);//设置SqlCommand永不超时
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>().Ignore(a => a.Name);                               // 忽略该字段
            // modelBuilder.Entity<User>().Ignore(a => a.Surname);                            // 忽略该字段
            // modelBuilder.Entity<User>().Property(a => a.EmailAddress).IsRequired(false);   // 设置可空

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
            // MemberUserConfiguration 等

            modelBuilder.ApplyConfiguration(new HomeInfoCfg());

            // [NotMapped] 
        }
    }
}
