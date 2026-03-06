using SaaS.Application.Common;
using SaaS.Application.Identity;

namespace SaaS.Application.Contracts;

public interface IIdentityService
{
    Task<ApplicationResult<SignInResponse>> SignInAsync(SignInIdentityCommand command, CancellationToken cancellationToken);
}
