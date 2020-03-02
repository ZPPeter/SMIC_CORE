
using DapperExtensions.Mapper;

namespace SMIC.Members
{
    public class AbpUserMapper : ClassMapper<AbpUser>  // AutoClassMapper -> Error
    {
        public AbpUserMapper()
        {
            Table("AbpUsers");
            Map(x => x.RoleNames).Ignore();
            AutoMap();
        }
    }
}
