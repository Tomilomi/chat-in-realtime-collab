namespace Application.Interfaces;

public interface IChatNotificationService
{
    Task KickUserAsync(string userId, string reason);
    Task NotifyMessageDeletedAsync(Guid messageId);
}