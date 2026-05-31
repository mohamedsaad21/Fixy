namespace Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;

public class CashCommissionItem
{
    public Guid BookingId { get; set; }
    public string CustomerName { get; set; }
    public decimal BookingAmount { get; set; }
    public decimal AmountOwed { get; set; }
    public DateTimeOffset BookingDate { get; set; }
}
