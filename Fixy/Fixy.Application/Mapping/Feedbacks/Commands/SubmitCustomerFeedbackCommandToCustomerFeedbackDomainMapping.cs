using Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;
using Fixy.Domain.Entities.Feedback;

namespace Fixy.Application.Mapping.Feedbacks.Commands;

public static class SubmitCustomerFeedbackCommandToCustomerFeedbackDomainMapping
{
    public static CustomerFeedback ToCustomerFeedbackDomain(this SubmitCustomerFeedbackCommand command, Guid CustomerId, Guid TechnicianId)
    {
        return new CustomerFeedback
        {
            ServiceBookingId = command.BookingId,
            CustomerId = CustomerId,
            TechnicianId = TechnicianId,
            JobDurationMinutes = command.JobDurationMinutes,
            ResponseTimeMinutes = command.ResponseTimeMinutes,
            PunctualityRating = command.PunctualityRating,
            ProfessionalismRating = command.ProfessionalismRating,
            CommunicationRating = command.CommunicationRating,
            CleanlinessRating = command.CleanlinessRating,
            SatisfactionScore = command.SatisfactionScore,
            CostSatisfaction = command.CostSatisfaction,
            WeatherCondition = command.WeatherCondition,
            SubmittedAt = DateTime.UtcNow
        };
    }
}
