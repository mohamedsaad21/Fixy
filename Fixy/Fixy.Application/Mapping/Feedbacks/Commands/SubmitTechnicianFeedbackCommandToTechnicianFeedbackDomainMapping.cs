using Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;
using Fixy.Domain.Entities.Feedback;

namespace Fixy.Application.Mapping.Feedbacks.Commands;

public static class SubmitTechnicianFeedbackCommandToTechnicianFeedbackDomainMapping
{
    public static TechnicianFeedback ToTechnicianFeedbackDomain(this SubmitTechnicianFeedbackCommand command, Guid customerId, Guid TechnicianId)
    {
        return new TechnicianFeedback
        {
            ServiceBookingId = command.BookingId,
            CustomerId = customerId,
            TechnicianId = TechnicianId,
            CustomerBehaviorRating = command.CustomerBehaviorRating,
            ClarityOfIssue = command.ClarityOfIssue,
            SafetyAndEnvironment = command.SafetyAndEnvironment,
            CustomerPunctuality = command.CustomerPunctuality,
            SubmittedAt = DateTime.UtcNow,
        };
    }
}
