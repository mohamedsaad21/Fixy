namespace Fixy.Application.Features.Payments.Queries.GetPendingCommissions.Responses;

public class GetPendingCommissionsResponse
{
    public decimal TotalOwed { get; set; }
    public int CommissionCount { get; set; }
    public List<CommissionItem> Commissions { get; set; }
}
