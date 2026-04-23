using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Chat;

public class Conversation : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ApplicationUser Customer { get; set; }
    public Guid TechnicianId { get; set; }
    public ApplicationUser Technician { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; }
    public bool IsClosed { get; set; }
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
}