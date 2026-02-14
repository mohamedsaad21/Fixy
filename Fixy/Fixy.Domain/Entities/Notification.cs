using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fixy.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    [NotMapped]
    public object Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
