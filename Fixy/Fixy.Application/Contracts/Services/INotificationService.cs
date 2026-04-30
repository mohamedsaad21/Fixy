using Fixy.Domain.Enums;

namespace Fixy.Application.Contracts.Services;

public interface INotificationService
{
    Task SaveNotificationAsync(Guid userId, NotificationType type, string titleKey, string bodyKey);
    Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
    Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}