using SaaS.Application.Common;

namespace SaaS.Application.User;

public interface IInviteUserCommandHandler
{
    Task<ApplicationResult<InviteUserResponse>> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken);
}
