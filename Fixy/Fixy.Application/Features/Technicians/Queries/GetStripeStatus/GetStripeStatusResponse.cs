namespace Fixy.Application.Features.Technicians.Queries.GetStripeStatus;

public class GetStripeStatusResponse
{
    public bool IsOnboarded {  get; set; }
    public bool CanReceivePayments {  get; set; }
    public string Status { get; set; }
}
