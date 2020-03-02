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
using Abp.Domain.Uow;
using SMIC.SDIM.Dtos;
using Abp.Specifications;

namespace SMIC.SDIM
{
    [AbpAuthorize]
    public class SJMXAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<JDRQ, long> _jdrqDapperRepository;
        private readonly IDapperRepository<SJMX, long> _sjmxDapperRepository;
        private readonly IDapperRepository<RecentSJMX, long> _sjmxRecentDapperRepository;        
        private readonly IDapperRepository<STATS, long> _DapperRepository;
        private readonly ICacheManager _cacheManager;//依赖注入缓存

        private readonly CZRZAppServices _czrzAppServices;//依赖注入缓存


        public SJMXAppServices(IDapperRepository<SJMX, long> sjmxDapperRepository, IDapperRepository<JDRQ, long> jdrqDapperRepository, ICacheManager cacheManager,IDapperRepository<STATS, long> DapperRepository, IDapperRepository<RecentSJMX, long> sjmxRecentDapperRepository, CZRZAppServices czrzAppServices) 
        {
            _jdrqDapperRepository = jdrqDapperRepository;
            _sjmxDapperRepository = sjmxDapperRepository;
            _cacheManager = cacheManager;//依赖注入缓存
            _DapperRepository = DapperRepository;
            _czrzAppServices = czrzAppServices;
            _sjmxRecentDapperRepository = sjmxRecentDapperRepository;
        }

        public SJMX Get(int id)
        {
            string strSQL = @"SELECT a.ID, e.sjdid ,e.djrq ,d.QJMC , b.XHGGMC , a.ccbh ,a.jdzt
                            FROM dbo.YQSF_SJMX AS a INNER JOIN											
                            dbo.YQSF_SJD as e on a.sjdid = e.id INNER JOIN
                            dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
                            dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM
                            where a.ID={0}";
            strSQL = string.Format(strSQL, id);
            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            if (ret != null)
                return ret.FirstOrDefault();
            else
                return null;
        }
        public dynamic GetLastSJMX() {
            string strSQL = @"select top 3 * from  VW_DPII_SJD Order By ID Desc";
            IEnumerable<RecentSJMX> ret = _sjmxRecentDapperRepository.Query(strSQL);
            return ret;
        }
        public dynamic GetRecentSJMX()
        {
            string strSQL = @"SELECT TOP (10) a.ID, e.sjdid ,e.djrq ,d.QJMC , b.XHGGMC , a.ccbh ,a.jdzt
                            FROM dbo.YQSF_SJMX AS a INNER JOIN											
                            dbo.YQSF_SJD as e on a.sjdid = e.id INNER JOIN
                            dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
                            dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM
                            ORDER BY a.ID DESC";
            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            return ret;
        }

        public dynamic GetRecentSJMXBy(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return GetRecentSJMX();
            string strSQL = @"
SELECT top(10) * FROM (
SELECT * FROM (SELECT TOP (10) a.ID, e.sjdid ,e.djrq ,d.QJMC , b.XHGGMC , a.ccbh ,a.jdzt
                            FROM dbo.YQSF_SJMX AS a  RIGHT JOIN
														dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
                            dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
                            dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM                            
                            where a.ccbh like '%{0}%'
							order by a.id desc				
							) as a 
union all
SELECT * FROM (SELECT TOP (10) a.ID, e.sjdid ,e.djrq ,d.QJMC , b.XHGGMC , a.ccbh ,a.jdzt
                            FROM dbo.YQSF_SJMX AS a  RIGHT JOIN
														dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
                            dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
                            dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM                            
                            where e.sjdid like '%{0}%'
							order by a.id desc
														
) as b) as bb";

            strSQL = string.Format(strSQL, q.Trim());
            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            return ret;
        }        
        
        //委托单下面的List Sjmx
        public IEnumerable<SJMX> GetSjmxs(string sjdid)
        {
            string strSQL = @"
SELECT a.sjdid,qjmc,djrq,xhggmc,b.xhggbm,ccbh,zzcnr as zzc,jdy,a.bzsm,g.jdrq,g.jwrq,f.dwmc as wtdw,DATEADD(day,14,djrq) as yqjcrq,a.jdzt as jdzt1,g.jdzt as jdzt2
FROM dbo.YQSF_SJMX AS a LEFT JOIN
dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
dbo.YQSF_KH as f on e.khid = f.khid LEFT JOIN
dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
dbo.JCXX_ZZC_BM AS c ON b.ZZCBM = c.ZZCBM LEFT JOIN
dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM LEFT JOIN
dbo.SJCL_CHYQ as g on g.id = a.id 											
where d.QJMCBM = 1000 and e.djrq>'2019-04-21' and a.sjdid ='{0}'
";
            strSQL = string.Format(strSQL, sjdid);
            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);

            /* 前端处理            
                    string jdzt = reader["jdzt"].ToString();
                    string jdzt2 = reader["检定状态"].ToString();
                    if (jdzt2 == "检完")
                    {
                        sjmx.jdzt = 222;
                    }
                    else if (jdzt != "")
                        sjmx.jdzt = Int32.Parse(jdzt);
                    sjmx.img = GetZzcImg(zzc);
                    sjmx.jbcs = jbcs.GetJbcs(reader["xhggbm"].ToString());
           */
                    
            return ret;
        }

        
        //Search
        public IEnumerable<SJMX> ListSjmxs(string lb, string q, string userId)
        {

            string strSQL = @"select top 10 a.id,a.sjdid,qjmc,djrq,xhggmc,b.xhggbm,ccbh,zzcnr as zzc,jdy,a.bzsm,g.jdrq,g.jwrq,f.dwmc as wtdw,DATEADD(day, 14, djrq) as yqjcrq,a.jdzt as jdzt1,
        g.jdzt as jdzt2,e.sjdid as wtdh
        FROM dbo.YQSF_SJMX AS a LEFT JOIN
        dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
        dbo.YQSF_KH as f on e.khid = f.khid LEFT JOIN
        dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
        dbo.JCXX_ZZC_BM AS c ON b.ZZCBM = c.ZZCBM LEFT JOIN
        dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM LEFT JOIN
        dbo.SJCL_CHYQ as g on g.id = a.id
        where d.QJMCBM = 1000 and e.djrq > '2019-04-21'";                             // 查询searchAll
                        
            if (lb == "2")
                strSQL += " and  g.jdzt is null and a.jdzt<>'检完'";                  // 待检:JDZT is null
            else if (lb == "3")
                strSQL += " and  g.jdzt=122 and a.jdzt<>'检完' and jdy<>" + userId;   // 待核验
            else if (lb == "4")
                strSQL += " and  g.jdzt=200 and a.jdzt<>'检完' ";                     // 待批准
            else if (lb == "9")
                strSQL += " and (g.jdzt<122 or g.jdzt IS NULL) and a.jdzt <>'检完' "; // 包含在检

            if (q != "")
            {
                strSQL += " and (ccbh like '%" + q + "%' or xhggmc like '%" + q + "%'";
                if (lb == "9")
                {
                    strSQL += " or f.dwmc like '%" + q + "%'";
                    strSQL += " or e.sjdid like '%" + q + "%'";
                }
                strSQL += ")";
            }
            if (lb == "9")
                strSQL += " ORDER BY a.ID DESC";

            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            return ret;
        }

        /*
        // 待检定列表 - SJCLAppServices/GetPagedDjmxs
        public IEnumerable<SJMX> ListDjmxs(string q)
        {
            string strSQL = @"select top 20 a.id,a.sjdid,qjmc,djrq,xhggmc,b.xhggbm,ccbh,zzcnr as zzc,jdy,a.bzsm,g.jdrq,g.jwrq,f.dwmc as wtdw,DATEADD(day, 14, djrq) as yqjcrq,a.jdzt as jdzt1,
        g.jdzt as jdzt2,e.sjdid as wtdh
        FROM dbo.YQSF_SJMX AS a LEFT JOIN
        dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
        dbo.YQSF_KH as f on e.khid = f.khid LEFT JOIN
        dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
        dbo.JCXX_ZZC_BM AS c ON b.ZZCBM = c.ZZCBM LEFT JOIN
        dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM LEFT JOIN
        dbo.SJCL_CHYQ as g on g.id = a.id
        where d.QJMCBM = 1000 and e.djrq > '2019-04-21'";                     // 查询searchAll

        strSQL += " and  g.jdzt is null and a.jdzt<>'检完'";                  // 待检:JDZT is null

            if (q != "")
            {
                strSQL += " and (ccbh like '%" + q + "%' or xhggmc like '%" + q + "%'";
                strSQL += ")";
            }

            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            return ret;
        }
        
        public async Task<PagedResultDto<SJMXListDto>> GetPagedSjmxs(GetVwSjmxsInput input)
        {
            Expression<Func<SJMX, bool>> predicate = p => (p.Id != 1);

            var totalCount = _sjmxDapperRepository.Count(predicate);


            var entityList = await _sjmxDapperRepository.GetAll()
                .OrderByDescending(t => t.Id)
                .PageBy(input)
                .ToListAsync();
            var entityListDtos = ObjectMapper.Map<List<SJMXListDto>>(entityList);

            return new PagedResultDto<SJMXListDto>(totalCount, entityListDtos);
        }        
        */

        public PagedResultDto<SJMX> GetPagedSjmxs(GetVwSjmxsInput input)
        {
            // 数据库里面必须有 SJMX 实体或者视图
            // Expression<Func<SJMX, bool>> predicate = p => p.qjmc == "全站仪";
            Expression<Func<SJMX, bool>> predicate = p => p.Id > 0;

            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(p => (p.xhggmc.Contains(input.FilterText) || p.ccbh.Contains(input.FilterText) || p.wtdh.Contains(input.FilterText) || p.wtdw.Contains(input.FilterText) ));
            }                      

            var totalCount = _sjmxDapperRepository.Count(predicate); 
            IEnumerable<SJMX> ret = _sjmxDapperRepository.GetAllPaged(
                predicate,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<SJMX> tempList2 = ObjectMapper.Map<List<SJMX>>(ret);
            return new PagedResultDto<SJMX>(
                totalCount,
                tempList2
            );
        }

        /*
        public dynamic GetSjmx1()
        {
            dynamic ret = _sjmxDapperRepository.GetAllPaged(x => x.qjmc == "全站仪", 0, 20, "ID").ToDynamicList<dynamic>();
            return ret;
        }*/

        public IEnumerable<SJMX> GetSjmxBySjdid(string sjdid)
        {
            Expression<Func<SJMX, bool>> predicate = p => p.sjdid == sjdid;
            IEnumerable<SJMX> ret = _sjmxDapperRepository.GetAll(predicate);
            return ret;
        }

        //Search By WTDH
        public IEnumerable<SJMX> GetSjmxByWtdh(string q) // ???
        {
            string strSQL = @"
        select top 20 a.sjdid,e.sjdid as wtdh,qjmc,djrq,xhggmc,b.xhggbm,ccbh,zzcnr as zzc,jdy,a.bzsm,g.jdrq,g.jwrq,f.dwmc as wtdw,DATEADD(day, 14, djrq) as yqjcrq,a.jdzt as jdzt1,g.jdzt as jdzt2
        FROM dbo.YQSF_SJMX AS a LEFT JOIN
        dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
        dbo.YQSF_KH as f on e.khid = f.khid LEFT JOIN
        dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
        dbo.JCXX_ZZC_BM AS c ON b.ZZCBM = c.ZZCBM LEFT JOIN
        dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM LEFT JOIN
        dbo.SJCL_CHYQ as g on g.id = a.id
        where d.QJMCBM = 1000 and e.djrq > '2019-04-21' and e.sjdid='" + q + "'";

            IEnumerable<SJMX> ret = _sjmxDapperRepository.Query(strSQL);
            return ret;
        }
    
        // public static List<int> GetMonthCount(int year) -> StatsAppServices.cs
              

        //重新检定
        public int ResetJdzt(string id)
        {
            string strSQL = "update YQSF_SJD set jdzt='在检' where ID = (select sjdid from YQSF_SJMX as b where b.id = " + id + ")";
            _sjmxDapperRepository.Execute(strSQL);
            strSQL = "Update YQSF_SJMX set jdzt='登记' where id=" + id;
            _sjmxDapperRepository.Execute(strSQL);
            strSQL = "Update SJCL_CHYQ set JDRQ=null,jwrq=null,JDYID=null,hyy=null,HYYID=null,PZRID=null,HYYJ='',PZYJ='',PZR='',jdzt=100 where id=" + id;
            _sjmxDapperRepository.Execute(strSQL);

            CZRZDto czrz = new CZRZDto();
            czrz.CZNR = "重新检定："+id;
            _czrzAppServices.AddtoCZRZ(czrz);
            return 1;
        }

        // 出厂编号修改
        public int UpdateCcbh(UpdateCcbhDto input) //int id, string ccbh)
        {
            CZRZDto czrz = new CZRZDto();
            czrz.CZNR = "修改 "+ input.ID+" 出厂编号：" + input.OCcbh + "->" + input.Ccbh;
            czrz.BZSM = input.BZSM;
            _czrzAppServices.AddtoCZRZ(czrz);

            string strSQL = "update YQSF_SJMX set ccbh = '" + input.Ccbh + "' where id = " + input.ID;
            return _sjmxDapperRepository.Execute(strSQL);
        }
         
        /*
        public string GetSjmxJdzt(int id) {
            string jdzt0 = "";
            int? jdzt1 = null;
            SJMX sjmx = Get(id);
            if (sjmx != null)
                jdzt0 = sjmx.jdzt1;
                        
            JDRQ jdrq = _jdrqAppServices.Get(id);
            //JDRQ jdrq = GetJdrq(id);
            if (jdrq != null)
                jdzt1 = jdrq.jdzt;

            if (jdzt1 != null)
                jdzt0 += jdzt1.ToString();

            if (jdzt0.IndexOf("222") > 0)
            {
                _jdrqAppServices.SetOverByJdzt(id, 222);
            }

            return jdzt0;            
        }
        */

        /*
        public void CheckSjmxJdzt(int id)
        {
            string strSQL = "select 0 as y, 0 as m, q z yjs as count, id as bm from YQSF_SJD where ID = (select sjdid from YQSF_SJMX as b where b.id = {0}";
            STATS st = _DapperRepository.Query(string.Format(strSQL, id)).FirstOrDefault();
            int yqjs = st.count;
            int wtdid = st.bm;
            strSQL = @"SELECT 0 as y, 0 as m, q z yjs as count, 0 as bm
        FROM dbo.YQSF_SJMX AS a LEFT JOIN
        dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
        dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
        dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM								
        where d.QJMCBM = 1000 and e.djrq>'2019-04-21' and jdzt=222 and e.sjdid={0}";
            st = _DapperRepository.Query(string.Format(strSQL, wtdid)).FirstOrDefault();
            int yqjs1 = st.count;

            strSQL = "select 0 as y, 0 as m, count(0) as count,0 as bm from YQSF_SJMX where jdzt = '检完' and ID = {0}";
            st = _DapperRepository.Query(string.Format(strSQL, id)).FirstOrDefault();
            int yqjs2 = st.count;

            if (yqjs == yqjs1 || yqjs == yqjs2) {
                strSQL = "update YQSF_SJD set q z yjdzt = N'检定完毕' where id = {0}";                
                st = _DapperRepository.Query(string.Format(strSQL, wtdid)).FirstOrDefault();
            }
        } */      
    }
}