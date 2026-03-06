using SaaS.Application.Common;

namespace SaaS.Application.Identity;

public interface ILoginCommandHandler
{
    Task<ApplicationResult<SignInResponse>> HandleAsync(SignInIdentityCommand command, CancellationToken cancellationToken);
}
