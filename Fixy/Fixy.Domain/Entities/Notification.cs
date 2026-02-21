using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
