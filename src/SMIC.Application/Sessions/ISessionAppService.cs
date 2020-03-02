using System.Threading.Tasks;
using Abp.Application.Services;
using SMIC.Sessions.Dto;

namespace SMIC.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
