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
using Abp.Specifications;
using System;
using SMIC.SDIM.Dtos;
namespace SMIC.SDIM
{
    public class VWSJMXAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<VW_SJMX, long> _vwsjmxDapperRepository;
        public VWSJMXAppServices(IDapperRepository<VW_SJMX, long> vwsjmxDapperRepository) {
            _vwsjmxDapperRepository = vwsjmxDapperRepository;
        }

        //public IEnumerable<VW_SJMX> GetSjmx()
        //public List<VW_SJMX> GetSjmx()

        public dynamic GetSjmx1()
        {
            // Pages 从 0 开始
            dynamic ret = _vwsjmxDapperRepository.GetAllPaged(x => x.器具名称 == "全站仪", 0, 20, "ID").ToDynamicList<dynamic>(); //OK
            //IEnumerable<VW_SJMX> ret = _vwsjmxDapperRepository.Query("select top 20 * from VW_SJMX"); //OK                        
            return ret;
        }

        public int GetVW_SJMXCount()
        {
            Expression<Func<VW_SJMX, bool>> predicate = p => p.器具名称 == "全站仪";
            predicate = predicate.And(p => p.单位名称.Contains("济南"));
            predicate = predicate.And(p => p.送检日期 >= DateTime.Parse("2005-12-21"));
            return _vwsjmxDapperRepository.Count(predicate);
        }

        public PagedResultDto<VW_SJMX> GetPagedVwSjmxs(GetVwSjmxsInput input)
        {
            /* 方法一
            Expression condition = null;
            ParameterExpression param = Expression.Parameter(typeof(VW_SJMX), "p");
            Expression right = Expression.Constant("-1");
            Expression left = Expression.Property(param, typeof(VW_SJMX).GetProperty("送检单号"));
            Expression filter = Expression.NotEqual(left, right); // 送检单号 != -1
            condition = filter;

            right = Expression.Constant("12345");
            left = Expression.Property(param, typeof(VW_SJMX).GetProperty("送检单号"));
            filter = Expression.Equal(left, right);
            condition = Expression.And(condition, filter);            
            var predicate = Expression.Lambda<Func<VW_SJMX, bool>>(condition, param);
            */

            /* 方法二 */

            Expression<Func<VW_SJMX, bool>> predicate = p => p.器具名称 == "全站仪";

            //if (input.From != null) // DateTime? 会有问题
            if (input.From > DateTime.MinValue)
            {
                predicate = predicate.And(p => p.送检日期 >= DateTime.Parse(input.From.ToString())); // input.From
            }
            if (input.To > DateTime.MinValue) // != null
            {
                predicate = predicate.And(p => p.送检日期 <= DateTime.Parse(input.To.ToString()));   // input.To
            }

            //if (!input.Filter.IsNullOrWhiteSpace())
            if (!input.WTDH.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(p => p.送检单号.Contains(input.WTDH));
            }

            if (!input.WTDW.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(p => p.单位名称.Contains(input.WTDW));
            }

            var totalCount = _vwsjmxDapperRepository.Count(predicate);

            /* - 固定查询条件
            var totalCount1 = _vwsjmxDapperRepository.Count(
                a => (a.器具名称 == "全站仪") &&
                a.送检单号.Contains(input.FilterText) &&
                a.送检日期 >= input.From &&
                a.送检日期 <= input.To
                );
            //IEnumerable<VW_SJMX> ret = _vwsjmxDapperRepository.GetAllPaged(x => x.器具名称 == "全站仪", 2, 20, "ID");
            //IEnumerable<VW_SJMX> ret = _vwsjmxDapperRepository.GetAllPaged(a =>(a.器具名称 == "全站仪") && a.送检单号.Contains(input.FilterText), input.SkipCount/input.MaxResultCount, input.MaxResultCount, input.Sorting);
            */

            //Logger.Info("input.Filter.IsNullOrWhiteSpace():" + input.Filter.IsNullOrWhiteSpace());
            //Logger.Info("input.Filter.IsNullOrEmpty():" + input.Filter.IsNullOrEmpty());
            //Logger.Info("input.Filter == ''" + (input.Filter == ""));
            //Logger.Info(predicate.Body.ToString());

            //Logger.Info(predicate.Body.ToString());

            IEnumerable<VW_SJMX> ret = _vwsjmxDapperRepository.GetAllPaged(
                predicate,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "asc"); // input.Order=="asc"  true/false

            /*
            List<VW_SJMX> tempList = new List<VW_SJMX>();
            IEnumerator<VW_SJMX> currentEnumerator = ret.GetEnumerator();
            if (currentEnumerator != null)
            {
                for (int count = 0; currentEnumerator.MoveNext(); count++)
                {
                    tempList.Add(currentEnumerator.Current);
                }
            }
            */

            List<VW_SJMX> tempList2 =  ObjectMapper.Map<List<VW_SJMX>>(ret);
            return new PagedResultDto<VW_SJMX>(
                totalCount,
                tempList2
            );
        }

        //Search By WTDH
        public IEnumerable<VW_SJMX> GetSjmxBySjdid(double sjdid)
        {
            //Expression<Func<VW_SJMX, bool>> predicate = p => p.送检单号 == wtdh;
            Expression<Func<VW_SJMX, bool>> predicate = p => p.送检单ID == sjdid;
            IEnumerable<VW_SJMX> ret = _vwsjmxDapperRepository.GetAll(predicate);
            return ret;
        }
               
        /*
        public int GetSjmx3()
        {
            //Expression.IfThen()

            Expression condition = null;
            ParameterExpression param = Expression.Parameter(typeof(Person), "p");
            Expression right = Expression.Constant("Name");
            Expression left = Expression.Property(param, typeof(Person).GetProperty("Name"));
            Expression filter = Expression.Equal(left, right);
            condition = filter;
            condition = Expression.And(condition, filter);

            var totalCount = _personDapperRepository.Count(Expression.Lambda<Func<Person, bool>>(filter, param)); // ok

            return totalCount;
        }
        */

    }
}
