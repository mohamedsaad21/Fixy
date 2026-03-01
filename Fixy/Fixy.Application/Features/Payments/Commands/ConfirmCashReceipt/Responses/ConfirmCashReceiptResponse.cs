namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt.Responses;

public class ConfirmCashReceiptResponse
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal AmountConfirmed { get; set; }
    public decimal TechnicianEarning { get; set; }
    public decimal PlatformCommission { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
