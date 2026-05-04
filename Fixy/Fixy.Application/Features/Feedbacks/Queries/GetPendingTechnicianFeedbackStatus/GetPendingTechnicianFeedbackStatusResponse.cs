namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingTechnicianFeedbackStatus;

public class GetPendingTechnicianFeedbackStatusResponse
{
    public bool HasPendingFeedback { get; set; }
    public Guid? BookingId { get; set; }
}
