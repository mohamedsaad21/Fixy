namespace Fixy.Application.Features.Payments.Queries.GetPendingCommissions.Responses;

public class CommissionItem
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string CustomerName { get; set; }
    public string ServiceDescription { get; set; }
    public decimal AmountOwed { get; set; }
    public DateTime CreatedAt { get; set; }
}
