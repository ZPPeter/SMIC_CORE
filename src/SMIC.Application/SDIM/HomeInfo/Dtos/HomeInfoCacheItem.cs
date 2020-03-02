using Abp.AutoMapper;
namespace SMIC.SDIM
{
    [AutoMapFrom(typeof(HomeInfo))]
    public class HomeInfoCacheItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
