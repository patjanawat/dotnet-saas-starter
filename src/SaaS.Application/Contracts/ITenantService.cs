using SaaS.Application.Common;
using SaaS.Application.Tenant;

namespace SaaS.Application.Contracts;

public interface ITenantService
{
    Task<ApplicationResult<TenantResponse>> CreateTenantAsync(CreateTenantCommand command, CancellationToken cancellationToken);
}
