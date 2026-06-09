using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;

namespace Fixy.Application.Contracts.Services;

public interface INotificationService
{
    Task SendFullNotificationAsync(ApplicationUser user, NotificationType type, string titleKey, string bodyKey, Dictionary<string, string>? additionalData = null);
}