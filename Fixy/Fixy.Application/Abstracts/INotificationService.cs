namespace Fixy.Application.Abstracts;

public interface INotificationService
{
    Task SendNotificationToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
}