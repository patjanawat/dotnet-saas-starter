namespace SaaS.Api.Identity;

public sealed record LoginRequest(string UserNameOrEmail, string Password);

public sealed record LoginResponse(Guid UserId, string Email, string DisplayName);
