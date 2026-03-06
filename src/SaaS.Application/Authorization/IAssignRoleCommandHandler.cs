using SaaS.Application.Common;

namespace SaaS.Application.Authorization;

public interface IAssignRoleCommandHandler
{
    Task<ApplicationResult<AssignRoleResponse>> HandleAsync(AssignRoleCommand command, CancellationToken cancellationToken);
}
