namespace Fixy.Application.Contracts.Services;

public interface INotificationService
{
    Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
    Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}