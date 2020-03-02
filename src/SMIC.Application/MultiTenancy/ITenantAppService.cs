using Abp.Application.Services;
using SMIC.MultiTenancy.Dto;

namespace SMIC.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

