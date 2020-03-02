using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Abp.Specifications;
using System.Reflection;
using SMIC.SJCL;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using SMIC.SJCL.Common;
using System.Text;

using System.Diagnostics;
using System.Data;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Abp.Dapper.Filters;

using Abp.Timing; // Clock.Now;
using Newtonsoft.Json;
using DapperExtensions;
using Abp.Data;
using Abp.Runtime.Caching;
using SMIC.SDIM.Dtos;
using Microsoft.AspNetCore.Identity;
using SMIC.Authorization.Users;
namespace SMIC.SDIM
{
    public class CZRZAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<VW_CZRZ> _czrzDapperRepository;

        public CZRZAppServices(IDapperRepository<VW_CZRZ> czrzDapperRepository)
        {
            //Logger.Info("此处尚未初始化，不能运行");
            _czrzDapperRepository = czrzDapperRepository;
        }
        
        public PagedResultDto<VW_CZRZ> GetPagedCzrzs(GetCzrzInput input)
        {
            Expression<Func<VW_CZRZ, bool>> predicate = p => p.Id > 0;

            if (!input.isAdmin) {
                long jdyid = (long)AbpSession.UserId;
                predicate = predicate.And(p => p.UserID == jdyid);
            }
            
            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(p => (p.Surname==input.FilterText || p.CZNR.Contains(input.FilterText) ));
            }

            var totalCount = _czrzDapperRepository.Count(predicate);
            IEnumerable<VW_CZRZ> ret = _czrzDapperRepository.GetAllPaged(
                predicate,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<VW_CZRZ> tempList2 = ObjectMapper.Map<List<VW_CZRZ>>(ret);
            return new PagedResultDto<VW_CZRZ>(
                totalCount,
                tempList2
            );
        }

        public void AddtoCZRZ(CZRZDto czrz)
        {
            long jdyid = (long)AbpSession.UserId;
            var param = new
            {
                userid = jdyid,                
                cznr = czrz.CZNR,
                bzsm = czrz.BZSM
            };
            string strSQL = @"insert into SYS_CZRZ(userid,cznr,bzsm) values(@userid,@cznr,@bzsm)";
            int ret = _czrzDapperRepository.Execute(strSQL, param);
        }
    }
}
