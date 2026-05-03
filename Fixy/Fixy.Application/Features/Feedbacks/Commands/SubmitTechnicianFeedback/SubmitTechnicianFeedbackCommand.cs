using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public sealed record SubmitTechnicianFeedbackCommand
    (
        Guid BookingId,
        int CustomerBehaviorRating,
        int ClarityOfIssue,
        int SafetyAndEnvironment,
        int CustomerPunctuality
    ) : IRequest<Result>;
