using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Chat;

public class Conversation : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ApplicationUser Customer { get; set; }
    public Guid TechnicianId { get; set; }
    public ApplicationUser Technician { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastMessageAt { get; set; }
    public bool IsClosed { get; set; }
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
}