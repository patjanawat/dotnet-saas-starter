using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.Identity;
using SaaS.Domain.Modules.Identity;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class IdentityService : IIdentityService, ILoginCommandHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public IdentityService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<SignInResponse>> SignInAsync(SignInIdentityCommand command, CancellationToken cancellationToken)
        => await HandleAsync(command, cancellationToken);

    public async Task<ApplicationResult<SignInResponse>> HandleAsync(SignInIdentityCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserNameOrEmail) || string.IsNullOrWhiteSpace(command.Password))
        {
            return ApplicationResult<SignInResponse>.Failure("identity.invalid_input", "Username/email and password are required.", 400);
        }

        var normalized = command.UserNameOrEmail.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            x => x.NormalizedUserName == normalized.ToUpperInvariant() || x.NormalizedEmail == normalized.ToUpperInvariant(),
            cancellationToken);

        if (user is null)
        {
            return ApplicationResult<SignInResponse>.Failure("identity.invalid_credentials", "Invalid credentials.", 401);
        }

        var result = await _signInManager.PasswordSignInAsync(user, command.Password, true, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return ApplicationResult<SignInResponse>.Failure("identity.invalid_credentials", "Invalid credentials.", 401);
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var session = IdentitySessionRecord.Issue(user.Id, Guid.NewGuid().ToString("N"), now, now.AddHours(8));
        _dbContext.IdentitySessionRecords.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<SignInResponse>.Success(new SignInResponse(user.Id, user.Email ?? string.Empty, user.DisplayName));
    }
}
