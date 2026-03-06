using SaaS.Application.Common;

namespace SaaS.Application.Tenant;

public interface ICreateTenantCommandHandler
{
    Task<ApplicationResult<TenantResponse>> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken);
}
