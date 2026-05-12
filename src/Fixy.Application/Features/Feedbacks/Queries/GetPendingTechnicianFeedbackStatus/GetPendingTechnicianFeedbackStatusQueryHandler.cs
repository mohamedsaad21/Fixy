using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingTechnicianFeedbackStatus;

public sealed class GetPendingTechnicianFeedbackStatusQueryHandler : IRequestHandler<GetPendingTechnicianFeedbackStatusQuery, Result<GetPendingTechnicianFeedbackStatusResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFeedbackService _feedbackService;
    public GetPendingTechnicianFeedbackStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _feedbackService = feedbackService;
    }

    public async Task<Result<GetPendingTechnicianFeedbackStatusResponse>> Handle(GetPendingTechnicianFeedbackStatusQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await _currentUserService.GetCurrentUserId();

        var technician = await _unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
            return Errors.Unauthorized;

        var pendingTechnicianFeedbackBookingId = await _feedbackService.GetPendingTechnicianFeedbackBookingIdAsync(technician.Id);
        var response = new GetPendingTechnicianFeedbackStatusResponse
        {
            HasPendingFeedback = pendingTechnicianFeedbackBookingId != null,
            BookingId = pendingTechnicianFeedbackBookingId
        };
        return response;
    }
}
