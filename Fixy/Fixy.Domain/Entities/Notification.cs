using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public NotificationType Type { get; set; }
    public string TitleKey { get; set; }
    public string BodyKey { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
