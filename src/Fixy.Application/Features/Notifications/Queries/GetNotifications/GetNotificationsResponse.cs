namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string>? AdditionalData { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
