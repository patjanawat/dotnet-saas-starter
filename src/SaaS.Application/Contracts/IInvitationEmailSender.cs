using SaaS.Application.Notification;

namespace SaaS.Application.Contracts;

public interface IInvitationEmailSender
{
    Task SendInvitationAsync(InvitationEmailMessage message, CancellationToken cancellationToken);
}
