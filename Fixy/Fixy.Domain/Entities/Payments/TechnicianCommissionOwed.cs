namespace Fixy.Domain.Entities.Payments;

public class TechnicianCommissionOwed : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid BookingId { get; set; }
    public ServiceBooking Booking { get; set; }
    public decimal AmountOwed { get; set; }
    public bool IsPaid { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PaidAt { get; set; }
}
