namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public object Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
