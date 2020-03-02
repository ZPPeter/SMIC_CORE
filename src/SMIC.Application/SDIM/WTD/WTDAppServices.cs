using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Abp.Dapper.Repositories;
using System.Linq.Expressions;
using System.Linq;
using System;
using SMIC.SDIM.Dtos;
using Abp.Specifications;

namespace SMIC.SDIM
{
    [AbpAuthorize]
    public class WTDAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<WTD, long> _wtdDapperRepository;
        public WTDAppServices(IDapperRepository<WTD, long> wtdDapperRepository)
        {
            _wtdDapperRepository = wtdDapperRepository;
        }              
        
        public dynamic GetRecentWTD()
        {
            /*
            string strSQL = @"
            UPDATE YQSF_SJD
            SET q z yjs = (
                SELECT COUNT(*) FROM VW_SJMX WHERE[器具名称] = '全站仪' and 送检单号 = YQSF_SJD.sjdid
            )";
            _wtdDapperRepository.Execute(strSQL);
            */

            string strSQL = @"SELECT TOP(10) a.ID, a.SJDID , f.DWMC , a.SJRQ,YQJS
                              FROM dbo.YQSF_SJD AS a INNER JOIN 
                                   dbo.YQSF_KH AS f ON f.khid = a.khid
                              ORDER BY a.ID DESC";
            IEnumerable<WTD> ret = _wtdDapperRepository.Query(strSQL);
            return ret;
        }

        public dynamic GetRecentWTDBy(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return GetRecentWTD();
            string strSQL = @"select * from (select * from (SELECT TOP(10) a.ID, a.SJDID , f.DWMC , a.SJRQ,YQJS
                              FROM dbo.YQSF_SJD AS a INNER JOIN 
                                   dbo.YQSF_KH AS f ON f.khid = a.khid
                              where a.sjdid like '%{0}%'
                              ORDER BY a.ID DESC) as a
                              union
                              select * from (SELECT TOP(10) a.ID, a.SJDID , f.DWMC , a.SJRQ,YQJS
                              FROM dbo.YQSF_SJD AS a INNER JOIN 
                                   dbo.YQSF_KH AS f ON f.khid = a.khid
                              where f.dwmc like N'%{0}%'
                              ORDER BY a.ID DESC) as b) as c order by id desc";
            strSQL = string.Format(strSQL, q.Trim());
            IEnumerable<WTD> ret = _wtdDapperRepository.Query(strSQL);
            return ret;
        }

        public PagedResultDto<WTD> GetPagedWtds(GetWtdInput input)
        {
            // 数据库里面必须有 WTD 实体或者视图
            Expression<Func<WTD, bool>> predicate = p => p.Id > 0;
            
            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                //predicate = predicate.And(p => (p.sjdid==(input.FilterText) || p.dwmc.Contains(input.FilterText)));
                predicate = predicate.And(p => (p.sjdid.Contains(input.FilterText) || p.dwmc.Contains(input.FilterText)));
            }

            var totalCount = _wtdDapperRepository.Count(predicate);
            IEnumerable<WTD> ret = _wtdDapperRepository.GetAllPaged(
                predicate,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<WTD> tempList2 = ObjectMapper.Map<List<WTD>>(ret);
            return new PagedResultDto<WTD>(
                totalCount,
                tempList2
            );
        }

    }
}


