namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingCustomerFeedbackStatus;

public class GetPendingCustomerFeedbackStatusResponse
{
    public bool HasPendingFeedback { get; set; }
    public Guid? BookingId { get; set; }
}
