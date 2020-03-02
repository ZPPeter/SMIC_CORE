using Abp.Dapper.Repositories;
using Abp.Dependency;
using Abp.Domain.Repositories;

using SMIC.PhoneBooks.Persons;
using MyProjectVue.SMIC;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using DapperExtensions;
using Abp.Specifications;
using System.Reflection;

namespace MyProjectVue.SMIC
{
    public class PersonAppService : MyProjectVueAppServiceBase
    {
        private readonly IDapperRepository<Person> _personDapperRepository;
        private readonly IDapperRepository<AbpUsers> _userDapperRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IDapperRepository<VW_SJCL_100, int> _vwsjclDapperRepository;

        public PersonAppService(
            IRepository<Person> personRepository,
            IDapperRepository<Person> personDapperRepository,
            IDapperRepository<AbpUsers> userDapperRepository,
            IDapperRepository<VW_SJCL_100> vwsjclDapperRepository
            )
        {
            _personRepository = personRepository;
            _personDapperRepository = personDapperRepository;
            _userDapperRepository = userDapperRepository;
            _vwsjclDapperRepository = vwsjclDapperRepository;
        }
                
        public dynamic DoSomeStuff1() // 虽然 _personDapperRepository 是 Person，但 dynamic 是运行后判断类型，此处 OK
        {
            var people = _personDapperRepository.Query("select * from AbpUsers");
            return people;
        }

        public IEnumerable<Person> DoSomeStuff2()
        {
            var people = _personDapperRepository.Query("select * from Persons");
            return people;
        }

        public IEnumerable<AbpUsers> DoSomeStuff3()
        {
            var people = _userDapperRepository.Query("select * from AbpUsers");
            return people;
        }

        public int DoSomeStuff4() {
            //Expression<Func<AbpUsers, bool>> predicate = p => p.Id > 0 && p.Name.Contains("string") ;// && p.QJMCBM.IsIn(roles); // 有 IsIn 但不支持，或者没有 IsIn

            int[] ints1 = new int[] { 1, 3, 5, 7, 9 }; // 没有 Contains,  using System.Linq;
            string[] nameArr = { "Ha", "Hunter", "Tom", "Lily", "Jay", "Jim", "Kuku", "Locu" };// 没有 Contains,  using System.Linq;
            List<string> list = new List<string>(nameArr);
            //list.Contains("Hunter"); //OK
            //new Decimal[] { 85, 86, 88 }.Contains(88);
            List<int> vals = new List<int>() { 1, 3, 5 }; // 有 Contains 但不支持
            Console.WriteLine(vals.Contains(3)); // OK
            //Expression<Func<AbpUsers, bool>> predicate = p => p.Id >0 && vals.Contains(p.CreatorUserId); // 不支持
            //Expression<Func<AbpUsers, bool>> predicate = p => p.Id > 0 && p.EmailAddress.Contains("string"); // OK
            Expression<Func<AbpUsers, bool>> predicate = p => p.Id > 0;
            predicate = predicate.And(p => p.CreatorUserId == 1);
            predicate = predicate.Or(p => p.CreatorUserId == 3); //循环调用解决 in 

            return  _userDapperRepository.Count(predicate);
        }

        public int DoSomeStuff0()
        {
            string[] ints = new string[] { "1000", "1030", "1040" };
            Expression<Func<VW_SJCL_100, bool>> predicate;
            string  val = "";
            ParameterExpression param = Expression.Parameter(typeof(int));
            
            if (ints.Length > 0)
            {
                predicate = p => p.QJMCBM == ints[0];
                //foreach (int o in ints) {}
                for (int i = 1; i < ints.Length; i++)
                {                    
                    val = ints[i];
                    //System.Diagnostics.Debug.WriteLine(val);
                    //predicate = predicate.Or(p => p.QJMCBM == ints[i]); //会报错的 out if ...
                    //predicate = predicate.Or(p => p.QJMCBM == val); //不报错但结果不对
                    //if (i.Equals(1)) predicate = predicate.Or(p => p.QJMCBM == ints[1]); // 数字 OK
                    //if (i == 2) predicate = predicate.Or(p => p.QJMCBM.Equals(ints[2])); // 数字 Error
                    //predicate = predicate.Or(p => p.QJMCBM.Equals(ints[i]));//会报错的 out if ...
                    //predicate = predicate.Or(p => p.QJMCBM.Equals(val));//不报错但结果不对
                    if (i == 1) predicate = predicate.Or(p => p.QJMCBM.Equals(ints[1])); // 字符 OK
                    if (i == 2) predicate = predicate.Or(p => p.QJMCBM.Equals(ints[2]));
                    // ... ...

                }
                
                //predicate = predicate.Or(p => p.QJMCBM == ints[1]); // OK
                //predicate = predicate.Or(p => p.QJMCBM == ints[2]);
                predicate = predicate.And(p => p.JDZT == 100);
            }else
                predicate = p => p.JDZT == 100;

            var totalCount = _vwsjclDapperRepository.Count(predicate);
            System.Diagnostics.Debug.WriteLine(totalCount);

            // int 的 Equals 不能用！！！ 改为 string OK
            ints = new string[] { "1000", "1030" };
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

            // 先 and 再 or 也可以	
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
            
            //predicate = p => p.JDZT == 100;
            //predicate = predicate.And(p => p.QJMCBM == 1030);
            //totalCount = _vwsjclDapperRepository.Count(predicate);

            return totalCount;
        }

    }
       
}
