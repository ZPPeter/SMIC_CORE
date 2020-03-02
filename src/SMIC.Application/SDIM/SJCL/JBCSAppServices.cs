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
using Abp.Domain.Entities;
using Abp.Runtime.Caching;
using System.Text;
using SMIC.SDIM.Dtos;
namespace SMIC.SDIM
{
    public class JBCSAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<JBCS, long> _jbcsDapperRepository;
        private readonly ICacheManager _cacheManager;//依赖注入缓存

        public JBCSAppServices(IDapperRepository<JBCS, long> jbcsDapperRepository, ICacheManager cacheManager)
        {
            _jbcsDapperRepository = jbcsDapperRepository;
            _cacheManager = cacheManager;//依赖注入缓存
        }

        public JBCS GetJbcs(string bm)
        {
            string strSQL = "select top 1 * from JCXX_QZY_JDZB where xhggbm=" + bm;

            IEnumerable<JBCS> ret = _jbcsDapperRepository.Query(strSQL);
            if (ret != null)
                return ret.FirstOrDefault();
            else
                return null; // AddNew 
        }

        public int AddJbcs(AddJbcsDto jbcs)
        {            
            StringBuilder strSql = new StringBuilder();

            strSql.Append("insert into JCXX_QZY_JDZB(");
            strSql.Append("xhggbm,bcjda,bcjdb,cjjd,bcfw,axles");
            strSql.Append(")");
            strSql.Append(" values (");
            strSql.Append(jbcs.ID + ",");
            strSql.Append(jbcs.BCJDA + ",");
            strSql.Append(jbcs.BCJDB + ",");
            strSql.Append(jbcs.CJJD + ",");
            strSql.Append(jbcs.BCFW + ",");
            strSql.Append(jbcs.Axles);
            strSql.Append(")");
            //strSql.Append(";select @@IDENTITY");

            return _jbcsDapperRepository.Execute(strSql.ToString());

        }
    }
}