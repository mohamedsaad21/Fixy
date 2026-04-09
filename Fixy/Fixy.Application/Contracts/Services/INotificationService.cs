namespace Fixy.Application.Contracts.Services;

public interface INotificationService
{
    Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
}