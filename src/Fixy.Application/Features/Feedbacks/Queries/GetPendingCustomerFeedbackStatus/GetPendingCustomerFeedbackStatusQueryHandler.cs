using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingCustomerFeedbackStatus;

public sealed class GetPendingCustomerFeedbackStatusQueryHandler : IRequestHandler<GetPendingCustomerFeedbackStatusQuery, Result<GetPendingCustomerFeedbackStatusResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFeedbackService _feedbackService;    
    public GetPendingCustomerFeedbackStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _feedbackService = feedbackService;
    }

    public async Task<Result<GetPendingCustomerFeedbackStatusResponse>> Handle(GetPendingCustomerFeedbackStatusQuery request, CancellationToken cancellationToken)
    {
        var currentCustomerId = await _currentUserService.GetCurrentUserId();

        var customer = await _unitOfWork.Customers.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentCustomerId);

        if (customer == null)
            return Errors.Unauthorized;

        var pendingCustomerFeedbackBookingId = 
            await _feedbackService.GetPendingCustomerFeedbackBookingIdAsync(customer.Id);

        var response = new GetPendingCustomerFeedbackStatusResponse
        {
            HasPendingFeedback = pendingCustomerFeedbackBookingId != null,
            BookingId = pendingCustomerFeedbackBookingId
        };
        return response;
    }
}
