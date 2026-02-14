namespace Fixy.Application.Common.DTOs;

public class PaymentIntentResultDto
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal TechnicianAmount { get; set; }
}
