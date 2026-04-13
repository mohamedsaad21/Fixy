using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;

public sealed record SubmitCustomerFeedbackCommand
    (
        Guid BookingId,
        int JobDurationMinutes,
        int ResponseTimeMinutes,
        int PunctualityRating,
        int ProfessionalismRating,
        int CommunicationRating,
        int CleanlinessRating,
        int SatisfactionScore,
        int CostSatisfaction,
        WeatherCondition WeatherCondition,
        string? TextComplain
    ) : IRequest<Result>;