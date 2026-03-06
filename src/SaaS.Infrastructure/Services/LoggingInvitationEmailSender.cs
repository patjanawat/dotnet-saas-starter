using Microsoft.Extensions.Logging;
using SaaS.Application.Contracts;
using SaaS.Application.Notification;

namespace SaaS.Infrastructure.Services;

public sealed class LoggingInvitationEmailSender : IInvitationEmailSender
{
    private readonly ILogger<LoggingInvitationEmailSender> _logger;

    public LoggingInvitationEmailSender(ILogger<LoggingInvitationEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendInvitationAsync(InvitationEmailMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Invitation hook invoked for tenant={TenantId} userProfile={UserProfileId}",
            message.TenantId,
            message.UserProfileId);

        return Task.CompletedTask;
    }
}
