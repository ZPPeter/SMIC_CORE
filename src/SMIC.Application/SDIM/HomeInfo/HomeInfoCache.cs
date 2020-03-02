using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMIC.SDIM
{
    public class HomeInfoCache : EntityCache<HomeInfo, HomeInfoCacheItem>, IHomeInfoCache, ITransientDependency
    {
        public HomeInfoCache(ICacheManager cacheManager, IRepository<HomeInfo> repository) : base(cacheManager, repository)
        {

        }

    }
}
