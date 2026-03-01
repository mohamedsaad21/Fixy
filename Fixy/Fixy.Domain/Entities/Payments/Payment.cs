using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid? ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    // Amounts
    public decimal TotalAmount { get; set; }
    public decimal TechnicianAmount { get; set; }
    public decimal PlatformCommission { get; set; }
    // Paymob details
    public string? PaymobOrderId { get; set; }
    public string? PaymobTransactionId { get; set; }
    public string? MerchantOrderId { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
