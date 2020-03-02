using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace SMIC.Controllers
{
    public abstract class SMICControllerBase: AbpController
    {
        protected SMICControllerBase()
        {
            LocalizationSourceName = SMICConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
