using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingCustomerFeedbackStatus;

public sealed record GetPendingCustomerFeedbackStatusQuery() : IRequest<Result<GetPendingCustomerFeedbackStatusResponse>>;