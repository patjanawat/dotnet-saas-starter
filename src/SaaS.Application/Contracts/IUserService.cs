using SaaS.Application.Common;
using SaaS.Application.User;

namespace SaaS.Application.Contracts;

public interface IUserService
{
    Task<ApplicationResult<InviteUserResponse>> InviteUserAsync(InviteUserCommand command, CancellationToken cancellationToken);
}
