namespace Fixy.Application.Common.DTOs.Notifications;

public record NotificationPayload
    (
        string Title,
        string Message,
        string Type,
        DateTimeOffset CreatedAt
    );