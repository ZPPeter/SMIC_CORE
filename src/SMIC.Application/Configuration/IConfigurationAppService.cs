using System.Threading.Tasks;
using SMIC.Configuration.Dto;

namespace SMIC.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
