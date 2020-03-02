using Abp.Dapper.Repositories;
using SMIC.Authorization.Roles;
using System;
using System.Collections.Generic;

namespace SMIC.SDIM
{
    public class RoleNameAppService : SMICAppServiceBase
    {
        private readonly IDapperRepository<RoleNames, long> _roleNamesRepository;
        private readonly IDapperRepository<Role, int> _roleRepository;

        public RoleNameAppService(IDapperRepository<RoleNames, long> roleNamesRepository, IDapperRepository<Role, int> roleRepository) {
            _roleNamesRepository = roleNamesRepository;
            _roleRepository = roleRepository;
        }
        private IEnumerable<RoleNames> GetRoles()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            long id = (long)AbpSession.UserId;
            var param = new { Id = id };
            var ret = _roleNamesRepository.Query("select b.id,b.DisplayName,b.NormalizedName from AbpRoles b where b.Id in (select RoleId from AbpUserRoles a where a.UserId = @Id)", param);
            return ret;
        }

        public List<string[]> GetRoleNames(long id) 
        {
            var param = new { Id = id };
            var ret = _roleNamesRepository.Query("select b.id,b.DisplayName,b.NormalizedName from AbpRoles b where b.Id in (select RoleId from AbpUserRoles a where a.UserId = @Id)", param);
            List<string[]> list = new List<string[]>();
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();
            foreach (RoleNames r in ret)
            {
                list1.Add(r.NormalizedName); // ADMIN， 1000
                list2.Add(r.DisplayName);    // 管理员，全站仪             
            }
            list.Add(list1.ToArray());
            list.Add(list2.ToArray());
            return list;
        }

        /*
        public string[] getDisplayName() {
            List<string> list = new List<string>();
            var ret = GetRoles();
            foreach (RoleNames r in ret)
            {
              list.Add(r.DisplayName);
            }
            return list.ToArray(); // string[]  
        }       
        */

        public string[] GetJDRolesList(long userid)
        {
            var param = new { Id = userid };
            var ret = _roleRepository.Query("select b.NormalizedName from AbpRoles b where b.Id in (select RoleId from AbpUserRoles a where a.UserId = @Id)", param);
            List<string> list = new List<string>();
            foreach (Role r in ret)
            {
                if (r.NormalizedName.StartsWith("1"))
                {
                    //System.Diagnostics.Debug.WriteLine(r.NormalizedName);
                    list.Add(r.NormalizedName);
                }
            }
            return list.ToArray(); // string[]            
        }

        /*
        // 1xxxx 1000 1010 ...
        public string GetJDRoles(long userid)
        {
            var param = new { Id = userid };
            var ret = _roleRepository.Query("select b.NormalizedName from AbpRoles b where b.Id in (select RoleId from AbpUserRoles a where a.UserId = @Id)", param);

            List<int> list = new List<int>();
            foreach (Role r in ret)
            {
                if (r.NormalizedName.StartsWith("1"))
                {
                    list.Add(int.Parse(r.NormalizedName));
                }
            }
            //return list.ToArray(); // string[]
            return String.Join(",", list.ToArray());
        }*/

    }
}
