using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Abp.UI;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;



using SMIC.SDIM.Dtos;
using SMIC.SDIM.DomainService;
using SMIC.SDIM.Authorization;

namespace SMIC.SDIM
{
    /// <summary>
    /// EntityCache 实体级别缓存，可自动维护，没有过期时间
    /// </summary>
    public class HomeInfoCacheApplicationService : SMICAppServiceBase
    {

        private readonly IHomeInfoCache _homeInfoCache;

        public HomeInfoCacheApplicationService(IRepository<HomeInfo, int> entityRopository
            , IHomeInfoCache homeInfoCache
      )
        {
            _homeInfoCache = homeInfoCache;
        }


        //如果查询为空，会报错，最好加个错误判断

        public dynamic GetHomeInfoById(int id)
        {
            try
            {
                return _homeInfoCache[id];
            }

            catch
            {

                return "";
            }

        }
    }
}
