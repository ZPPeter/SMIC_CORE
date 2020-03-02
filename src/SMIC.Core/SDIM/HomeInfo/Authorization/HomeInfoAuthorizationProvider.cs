

using System.Linq;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;
using SMIC.Authorization;

namespace SMIC.SDIM.Authorization
{
    /// <summary>
    /// 权限配置都在这里。
    /// 给权限默认设置服务
    /// See <see cref="HomeInfoPermissions" /> for all permission names. HomeInfo
    ///</summary>
    public class HomeInfoAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

		public HomeInfoAuthorizationProvider()
		{

		}

        public HomeInfoAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public HomeInfoAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

		public override void SetPermissions(IPermissionDefinitionContext context)
		{
			// 在这里配置了HomeInfo 的权限。
			//var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));
			//var administration = pages.Children.FirstOrDefault(p => p.Name == AppPermissions.Pages_Administration) ?? pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

			//var entityPermission = administration.CreateChildPermission(HomeInfoPermissions.Node , L("HomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.Query, L("QueryHomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.Create, L("CreateHomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.Edit, L("EditHomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.Delete, L("DeleteHomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.BatchDelete, L("BatchDeleteHomeInfo"));
			//entityPermission.CreateChildPermission(HomeInfoPermissions.ExportExcel, L("ExportExcelHomeInfo"));
		}

		private static ILocalizableString L(string name)
		{
			return new LocalizableString(name, SMICConsts.LocalizationSourceName);
		}
    }
}