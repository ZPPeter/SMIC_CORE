using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMIC.SDIM;
namespace SMIC.EntityFrameworkCore
{
    // 2. DbContext
    public class SDIMDbContext : AbpDbContext
    {        
        public virtual DbSet<AbpUsersEx> AbpUsersEx { get; set; }
        public virtual DbSet<RoleNames> RoleNames { get; set; }
        public virtual DbSet<VW_SJMX> VW_SJMX { get; set; }
        public virtual DbSet<VW_SJCL_100> VW_SJCL_100 { get; set; }

        public virtual DbSet<WTD> WTD { get; set; }
        
        public virtual DbSet<VW_CZRZ> VW_CZRZ { get; set; }

        public virtual DbSet<SJMX> SJMX { get; set; }

        public virtual DbSet<SJMX_ZSBH> SJMX_ZSBH { get; set; }

        public virtual DbSet<ZSH_DATA> ZSH_DATA { get; set; }

        public virtual DbSet<USER_NAME> USER_NAME { get; set; }
        
        public virtual DbSet<RecentSJMX> RecentSJMX { get; set; }
        
        public virtual DbSet<STATS> STATS { get; set; }
        public virtual DbSet<COUNT> COUNT { get; set; }

        public virtual DbSet<JBCS> JBCSS { get; set; }
        public virtual DbSet<JDRQ> JDRQS { get; set; }

        public SDIMDbContext(DbContextOptions<SDIMDbContext> options)
            : base(options)
        {
        }
    }
}