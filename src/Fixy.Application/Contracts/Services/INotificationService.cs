using Fixy.Domain.Enums;

namespace Fixy.Application.Contracts.Services;

public interface INotificationService
{
    Task SendFullNotificationAsync(Guid userId, NotificationType type, string titleKey, string bodyKey, Dictionary<string, string>? additionalData = null);
}