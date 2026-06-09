namespace Fixy.Domain.SP.TechnicianCashCommissionsOwed.Responses;

public class GetTechnicianCashCommissionsOwedResponse
{
    public decimal? TotalAmountOwed { get; set; }
    public int? BookingCount { get; set; }
    public List<CashCommissionItem>? Bookings { get; set; }
}
