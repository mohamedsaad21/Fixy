using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingCustomerFeedbackStatus;

public sealed class GetPendingCustomerFeedbackStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService, ILogger<GetPendingCustomerFeedbackStatusQueryHandler> logger) : IRequestHandler<GetPendingCustomerFeedbackStatusQuery, Result<GetPendingCustomerFeedbackStatusResponse>>
{
    public async Task<Result<GetPendingCustomerFeedbackStatusResponse>> Handle(GetPendingCustomerFeedbackStatusQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking pending customer feedback status. CustomerId: {CustomerId}", await currentUserService.GetCurrentUserId());

        var currentCustomerId = await currentUserService.GetCurrentUserId();

        var customer = await unitOfWork.Customers.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentCustomerId);

        if (customer == null)
        {
            logger.LogWarning("Pending customer feedback status check failed — customer not found. CustomerId: {CustomerId}", currentCustomerId);
            return Errors.Unauthorized;
        }

        var pendingCustomerFeedbackBookingId = 
            await feedbackService.GetPendingCustomerFeedbackBookingIdAsync(customer.Id);

        var hasPendingFeedback = pendingCustomerFeedbackBookingId != null;

        if (hasPendingFeedback)
            logger.LogInformation("Pending feedback found for customer. CustomerId: {CustomerId}, PendingBookingId: {PendingBookingId}", customer.Id, pendingCustomerFeedbackBookingId);
        else
            logger.LogInformation("No pending feedback for customer. CustomerId: {CustomerId}", customer.Id);

        return new GetPendingCustomerFeedbackStatusResponse
        {
            HasPendingFeedback = hasPendingFeedback,
            BookingId = pendingCustomerFeedbackBookingId
        };
    }
}
