
using System;
using Abp.Dapper.Repositories;
//using Abp.Domain.Repositories;

using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Specifications;
//using System.Reflection;
using SMIC.SDIM.Dtos;
using SMIC.Authorization.Roles;

using System.Linq;
//using System.Linq.Dynamic.Core;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using System.Reflection;

namespace SMIC.SDIM
{
    public class SJCLAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<COUNT, long> _cntDapperRepository; // 用 Dymaic 可以不用这个
        private readonly IDapperRepository<VW_SJCL_100, long> _vwsjclDapperRepository;
        private readonly RoleNameAppService _roleNameAppService;
        private readonly IDapperRepository<USER_NAME, long> _userNamesRepository;

        public SJCLAppServices(IDapperRepository<COUNT, long> cntDapperRepository, IDapperRepository<VW_SJCL_100, long> vwsjclDapperRepository, RoleNameAppService roleNameAppService, IDapperRepository<USER_NAME, long> userNamesRepository)
        {
            _cntDapperRepository = cntDapperRepository;
            _vwsjclDapperRepository = vwsjclDapperRepository;
            _roleNameAppService = roleNameAppService;
            _userNamesRepository = userNamesRepository;
        }
        /*
        检定状态：
        100      登记
        111      在检 JDYID
        122      检完确认 ->  待核验
        200      核验完毕 ->  生成证书，待批准
        222      批准完毕 ->  证书签名，检完
        */
        public double[] GetHomeCountData() // 待处理总任务、待检、在检、待核验、待批准
        {
            if (!AbpSession.UserId.HasValue)
                return new double[] { };

            long id = (long)AbpSession.UserId;

            //string roles = GetJDRoles(id);
            var roles = _roleNameAppService.GetJDRolesList(id); // [] 数组形式参数 结构化查询 

            //System.Diagnostics.Debug.WriteLine(id);
            //System.Diagnostics.Debug.WriteLine(roles);

            /*  
            //int[] ints = new int[] { 1000, 1030 };
            string[] ints = new string[] { "1000", "1030" };
            //Expression<Func<VW_SJCL_100, bool>> predicate = p => p.JDZT == 100 && p.QJMCBM.IsIn(ints); //The method 'p.QJMCBM.IsIn(value(System.Int32[]))' is not supported            
            Expression<Func<VW_SJCL_100, bool>> p1 = p => p.QJMCBM.Equals("1000");
            Expression<Func<VW_SJCL_100, bool>> p2 = p => p.QJMCBM.Equals("1030");
            Expression<Func<VW_SJCL_100, bool>> predicate = p1.Or(p2);

            //if(ints.Length>0)
            //predicate = predicate.And(p => p.QJMCBM == "1000"); //ints[0]
            //predicate = predicate.Or(p => p.QJMCBM.Equals("1030")); //ints[0]
            //for (int i=1;i<ints.Length;i++)
            //predicate = predicate.Or(p => p.QJMCBM == 1030);
            var totalCount = _vwsjclDapperRepository.Count(predicate); // 应该=8 实际=0 ！！！
            System.Diagnostics.Debug.WriteLine(totalCount);
                       

            //predicate = p => p.JDZT == 100;
            //totalCount = _vwsjclDapperRepository.Count(predicate);

            // int 的 Equals 不能用！！！ 改为 string OK
            // 2019-12-11 Abp.Dapper 4.8.1 无效 升级到 4.9 OK
            
            Expression<Func<VW_SJCL_100, bool>> newExp;
            if (ints.Length > 0)
            {
                ParameterExpression parameter = Expression.Parameter(typeof(VW_SJCL_100), "p");
                MemberExpression member = Expression.PropertyOrField(parameter, "QJMCBM");
                MethodInfo method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(ints[0], typeof(string));
                newExp = Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, constant), parameter);
                for (int i = 1; i < ints.Length; i++) { 
                    newExp = newExp.Or(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, Expression.Constant(ints[i], typeof(string))), parameter));
                }
                newExp = newExp.And(p => p.JDZT == 100);
            }
            else
                newExp = p => p.JDZT == 100;
            */

            /*
            // 先 and 再 or 	
            Expression<Func<VW_SJCL_100, bool>> newExp = p => p.JDZT == 100;
            if (ints.Length > 0)
            {
                ParameterExpression parameter = Expression.Parameter(typeof(VW_SJCL_100), "p");
                MemberExpression member = Expression.PropertyOrField(parameter, "QJMCBM");
                MethodInfo method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(ints[0], typeof(string));
                newExp = newExp.And(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, constant), parameter));
                for (int i = 1; i < ints.Length; i++)
                {
                    newExp = newExp.Or(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, Expression.Constant(ints[i], typeof(string))), parameter));
                }
            }
            var totalCount2 = _vwsjclDapperRepository.Count(newExp);
            System.Diagnostics.Debug.WriteLine(totalCount2);            
            */

            double cnt1 = 0;
            double cnt2 = 0;
            double cnt3 = 0;
            double cnt4 = 0;
            double cnt5 = 0;
            double cnt6 = 0;
            double cnt7 = 0; // 我的待检
            double cnt8 = 0; // 我的待核验

            // 字符串拼接
            // string userIds = "1,2,3,4";
            // strSQL = string.Format("select * from Users(nolock) where UserID in({0})", userIds); 
            //string strSQL = @"select count(0) as count from VW_SJCL_100 where jdzt = 122 and qjmcbm in (@roles) and jdyid !=@jdyid";
            //var param = new
            //{
            //   roles = roles,
            //};
            // cnt7 = _cntDapperRepository.Query(strSQL, param).FirstOrDefault().count; // where in 不能用此法

            // 参数化执行 where in

            string strSQL7 = @"
            declare @Temp_Variable varchar(max)
            create table #Temp_Table(Item varchar(max))
            while(LEN(@Temp_Array) > 0)
            begin
                if(CHARINDEX(',',@Temp_Array) = 0)
                begin
                    set @Temp_Variable = @Temp_Array
                    set @Temp_Array = ''
                end
                else
                begin
                    set @Temp_Variable = LEFT(@Temp_Array,CHARINDEX(',',@Temp_Array)-1)
                    set @Temp_Array = RIGHT(@Temp_Array,LEN(@Temp_Array)-LEN(@Temp_Variable)-1)
                end
                insert into #Temp_Table(Item) values(@Temp_Variable)
            end    
            select count(0) as count from VW_SJCL_100(nolock) where exists(select 1 from #Temp_Table(nolock) where VW_SJCL_100.jdzt<122 and #Temp_Table.Item=VW_SJCL_100.qjmcbm)
            drop table #Temp_Table";


            string strSQL8 = @"
            declare @Temp_Variable varchar(max)
            create table #Temp_Table(Item varchar(max))
            while(LEN(@Temp_Array) > 0)
            begin
                if(CHARINDEX(',',@Temp_Array) = 0)
                    begin
                        set @Temp_Variable = @Temp_Array
                        set @Temp_Array = ''
                    end
                else
                    begin
                        set @Temp_Variable = LEFT(@Temp_Array,CHARINDEX(',',@Temp_Array)-1)
                        set @Temp_Array = RIGHT(@Temp_Array,LEN(@Temp_Array)-LEN(@Temp_Variable)-1)
                    end
                insert into #Temp_Table(Item) values(@Temp_Variable)
            end    
            select count(0) as count from VW_SJCL_100(nolock) where exists(select 1 from #Temp_Table(nolock) where VW_SJCL_100.jdzt=@JDZT and VW_SJCL_100.jdyid!=@JDYID and #Temp_Table.Item=VW_SJCL_100.qjmcbm)
            drop table #Temp_Table";
            try
            {
                var param7 = new
                {
                    Temp_Array = roles
                };

                var param8 = new
                {
                    Temp_Array = roles,
                    JDZT = 122,
                    JDYID = id
                };

                try
                {
                    cnt7 = _cntDapperRepository.Query(strSQL7, param7).FirstOrDefault().count; //我的待检
                }
                catch
                {
                    cnt7 = 0;
                }
                try
                {
                    cnt8 = _cntDapperRepository.Query(strSQL8, param8).FirstOrDefault().count; //我的待核验
                }
                catch
                {
                    cnt8 = 0;
                }
                cnt1 = _cntDapperRepository.Query("select count(0) as count from VW_SJCL_100 where jdzt = 100").FirstOrDefault().count; //所有待检
                cnt2 = _cntDapperRepository.Query("select count(0) as count from VW_SJCL_100 where jdzt = 111").FirstOrDefault().count; //所有在检                
                cnt3 = _cntDapperRepository.Query("select count(0) as count from VW_SJCL_100 where jdzt = 122").FirstOrDefault().count; //所有待核验
                cnt4 = _cntDapperRepository.Query("select count(0) as count from VW_SJCL_100 where jdzt = 200").FirstOrDefault().count; //所有待批准
                cnt5 = _cntDapperRepository.Query("select count(0) as count from VW_SJMX where year([送检日期])= year(getdate())").FirstOrDefault().count;   //今年
                cnt6 = _cntDapperRepository.Query("select count(0) as count from VW_SJMX where year([送检日期])= year(getdate())-1").FirstOrDefault().count; //去年
            }
            catch
            {
                return new double[] { };
            }

            double[] Datas = new double[] { cnt1 + cnt2 + cnt3 + cnt4, cnt1, cnt2, cnt3, cnt4, cnt5, cnt6, cnt7, cnt8 };
            return Datas;
        }

        /*
        // 待检定列表
        public IEnumerable<VW_SJCL_100> ListDjmxs(string q)
        {
            if (!AbpSession.UserId.HasValue)
                return null;

            long id = (long)AbpSession.UserId;
            //var roles = GetJDRoles(id);

            var roles = String.Join(",", _roleNameAppService.GetJDRolesList(id).ToArray());            
            string strSQL = @"select * from VW_SJCL_100 where jdzt = 100 and qjmcbm in (" + roles + ")";

            if (!string.IsNullOrEmpty(q))
            {
                strSQL += " and (ccbh like '%" + q + "%' or xhggmc like '%" + q + "%'";
                strSQL += ")";
            }

            IEnumerable<VW_SJCL_100> ret = _vwsjclDapperRepository.Query(strSQL);
            return ret;
        }
        */

        /// <summary>
        /// 首页未完成列表
        /// JDZT<222
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultDto<VW_SJCL_100> GetPagedTasks(GetVwSjmxsInput input)
        {
            // 数据库里面必须有 SJMX 实体或者视图
            Expression<Func<VW_SJCL_100, bool>> predicate = p => p.Id > 0;

            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(p => (p.DWMC.Contains(input.FilterText) || p.ccbh.Contains(input.FilterText) || p.xhggmc.Contains(input.FilterText) || p.QJMC.Contains(input.FilterText)));
            }
            predicate = predicate.And(p => p.JDZT < 222);

            var totalCount = _vwsjclDapperRepository.Count(predicate);
            IEnumerable<VW_SJCL_100> ret = _vwsjclDapperRepository.GetAllPaged(
                predicate,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<VW_SJCL_100> tempList2 = ObjectMapper.Map<List<VW_SJCL_100>>(ret);
            return new PagedResultDto<VW_SJCL_100>(
                totalCount,
                tempList2
            );
        }

        /// <summary>
        /// 待检列表
        /// JDZT:100 111
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultDto<VW_SJCL_100> GetPagedDjmxs(GetVwSjmxsInput input)
        {
            if (!AbpSession.UserId.HasValue)
                return null;

            long id = (long)AbpSession.UserId;
            var roles = _roleNameAppService.GetJDRolesList(id);  //GetJDRolesList(id);

            Expression<Func<VW_SJCL_100, bool>> newExp = p => p.Id > 0;
            //Expression<Func<VW_SJCL_100, bool>> newExp = p => p.JDZT < 122;
            if (roles.Length > 0)
            {
                ParameterExpression parameter = Expression.Parameter(typeof(VW_SJCL_100), "p");
                MemberExpression member = Expression.PropertyOrField(parameter, "QJMCBM");
                MethodInfo method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(roles[0], typeof(string));
                newExp = newExp.And(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, constant), parameter));
                for (int i = 1; i < roles.Length; i++)
                {
                    newExp = newExp.Or(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, Expression.Constant(roles[i], typeof(string))), parameter));
                }
            }

            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                //r1,1000
                string[] split = input.FilterText.Split(new Char[] { ',' }); //返回:{"r1","1000"}
                if (split[0] == "r2")
                {
                    newExp = newExp.And(p => (p.JDZT == 100));//待检
                }
                if (split[0] == "r3")
                {
                    newExp = newExp.And(p => (p.JDZT == 111)); //在检 包含其他用户检定的，重检确认到综合查询
                }
                if (!split[1].IsNullOrWhiteSpace())
                    newExp = newExp.And(p => (p.QJMCBM == split[1]));
            }

            if (!input.WTDH.IsNullOrWhiteSpace())
            {
                newExp = newExp.And(p => (p.sjdid.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.DWMC.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.ccbh.Contains(input.WTDH)));
            }
            newExp = newExp.And(p => p.JDZT < 122);

            var totalCount = _vwsjclDapperRepository.Count(newExp);
            IEnumerable<VW_SJCL_100> ret = _vwsjclDapperRepository.GetAllPaged(
                newExp,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<VW_SJCL_100> tempList2 = ObjectMapper.Map<List<VW_SJCL_100>>(ret);
            return new PagedResultDto<VW_SJCL_100>(
                totalCount,
                tempList2
            );
        }

        /// <summary>
        /// 待检列表
        /// JDZT:100 111
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultDto<VW_SJCL_100> GetPagedDhymxs(GetVwSjmxsInput input)
        {
            if (!AbpSession.UserId.HasValue)
                return null;

            long id = (long)AbpSession.UserId;
            var roles = _roleNameAppService.GetJDRolesList(id);

            Expression<Func<VW_SJCL_100, bool>> newExp = p => p.Id > 0;
            if (roles.Length > 0)
            {
                ParameterExpression parameter = Expression.Parameter(typeof(VW_SJCL_100), "p");
                MemberExpression member = Expression.PropertyOrField(parameter, "QJMCBM");
                MethodInfo method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                ConstantExpression constant = Expression.Constant(roles[0], typeof(string));
                newExp = newExp.And(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, constant), parameter));
                for (int i = 1; i < roles.Length; i++)
                {
                    newExp = newExp.Or(Expression.Lambda<Func<VW_SJCL_100, bool>>(Expression.Call(member, method, Expression.Constant(roles[i], typeof(string))), parameter));
                }
            }

            if (!input.FilterText.IsNullOrWhiteSpace())
            {
                newExp = newExp.And(p => (p.QJMCBM == input.FilterText));
            }

            if (!input.WTDH.IsNullOrWhiteSpace())
            {
                newExp = newExp.And(p => (p.sjdid.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.DWMC.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.ccbh.Contains(input.WTDH)));
            }

            newExp = newExp.And(p => p.JDZT == 122);

            var totalCount = _vwsjclDapperRepository.Count(newExp);
            IEnumerable<VW_SJCL_100> ret = _vwsjclDapperRepository.GetAllPaged(
                newExp,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<VW_SJCL_100> tempList2 = ObjectMapper.Map<List<VW_SJCL_100>>(ret);
            return new PagedResultDto<VW_SJCL_100>(
                totalCount,
                tempList2
            );
        }

        /// <summary>
        /// 待批准列表
        /// JDZT:100 111
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PagedResultDto<VW_SJCL_100> GetPagedDpzmxs(GetVwSjmxsInput input)
        {
            if (!AbpSession.UserId.HasValue)
                return null;


            Expression<Func<VW_SJCL_100, bool>> newExp = p => p.Id > 0;

            if (!input.WTDH.IsNullOrWhiteSpace())
            {
                newExp = newExp.And(p => (p.sjdid.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.DWMC.Contains(input.WTDH)));
                newExp = newExp.Or(p => (p.ccbh.Contains(input.WTDH)));
            }

            newExp = newExp.And(p => p.JDZT == 200);

            var totalCount = _vwsjclDapperRepository.Count(newExp);
            IEnumerable<VW_SJCL_100> ret = _vwsjclDapperRepository.GetAllPaged(
                newExp,
                input.SkipCount / input.MaxResultCount,
                input.MaxResultCount,
                input.Sorting, input.Order == "desc"); // input.Order=="asc"  true/false
            List<VW_SJCL_100> tempList2 = ObjectMapper.Map<List<VW_SJCL_100>>(ret);
            return new PagedResultDto<VW_SJCL_100>(
                totalCount,
                tempList2
            );
        }

        /*
        public IEnumerable<RoleNames> GetRoles()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            //long id = (long)AbpSession.UserId;
            //var param = new { Id = id };
            var ret = _roleNameAppService.GetRoles(); //   _roleNamesRepository.Query("select b.id,b.DisplayName,b.NormalizedName from AbpRoles b where b.Id in (select RoleId from AbpUserRoles a where a.UserId = @Id)", param);
            return ret;
        }
        */

        public IEnumerable<USER_NAME> GetCacheUserData()
        {
            return _userNamesRepository.Query("select id,Surname from AbpUsers");
        }
    }
}

