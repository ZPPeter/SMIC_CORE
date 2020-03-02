using Abp.Domain.Entities;
namespace SMIC.SDIM
{
    public class RoleNames : Entity<long>
    {
        public string DisplayName { get; set; }
        public string NormalizedName { get; set; }

    }
}
