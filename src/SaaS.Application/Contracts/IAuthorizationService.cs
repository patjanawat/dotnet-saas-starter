using SaaS.Application.Authorization;
using SaaS.Application.Common;

namespace SaaS.Application.Contracts;

public interface IAuthorizationService
{
    Task<ApplicationResult<AssignRoleResponse>> AssignRoleAsync(AssignRoleCommand command, CancellationToken cancellationToken);
}
