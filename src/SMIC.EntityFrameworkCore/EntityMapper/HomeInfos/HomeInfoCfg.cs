

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMIC.SDIM;

namespace SMIC.EntityMapper.HomeInfos
{
    public class HomeInfoCfg : IEntityTypeConfiguration<HomeInfo>
    {
        public void Configure(EntityTypeBuilder<HomeInfo> builder)
        {
            //schema ָ�����������
            //builder.ToTable("HomeInfos", SMICConsts.SchemaNames.CMS);
            builder.ToTable("HomeInfos"); // Ĭ�� schema -> dbo             
            
			builder.Property(a => a.User).HasMaxLength(SMICConsts.EntityLengthNames.Length16);
			builder.Property(a => a.Title).HasMaxLength(SMICConsts.EntityLengthNames.Length32);
			builder.Property(a => a.Description).HasMaxLength(SMICConsts.EntityLengthNames.Length256);
			builder.Property(a => a.CreationTime).HasMaxLength(SMICConsts.EntityLengthNames.Length16);

        }
    }
}


