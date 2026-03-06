namespace SaaS.Application.Identity;

public sealed record SignInIdentityCommand(string UserNameOrEmail, string Password);

public sealed record SignInResponse(Guid UserId, string Email, string DisplayName);
