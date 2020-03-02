using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using SMIC.Configuration.Dto;

namespace SMIC.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : SMICAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
