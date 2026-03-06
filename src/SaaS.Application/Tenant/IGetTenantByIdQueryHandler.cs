using SaaS.Application.Common;

namespace SaaS.Application.Tenant;

public interface IGetTenantByIdQueryHandler
{
    Task<ApplicationResult<TenantResponse>> HandleAsync(GetTenantByIdQuery query, CancellationToken cancellationToken);
}
