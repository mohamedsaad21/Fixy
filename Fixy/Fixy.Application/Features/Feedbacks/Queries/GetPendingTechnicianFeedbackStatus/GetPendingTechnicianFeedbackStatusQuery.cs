using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingTechnicianFeedbackStatus;

public sealed record GetPendingTechnicianFeedbackStatusQuery() : IRequest<Result<GetPendingTechnicianFeedbackStatusResponse>>;