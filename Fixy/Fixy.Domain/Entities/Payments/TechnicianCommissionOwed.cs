namespace Fixy.Domain.Entities.Payments;

public class TechnicianCommissionOwed : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid BookingId { get; set; }
    public ServiceBooking Booking { get; set; }
    public decimal AmountOwed { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
