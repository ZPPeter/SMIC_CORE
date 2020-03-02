using Abp.Authorization;
using SMIC.Authorization.Roles;
using SMIC.Authorization.Users;

namespace SMIC.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
