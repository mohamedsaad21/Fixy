using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Api.Filters;

public class TechnicianFeedbackFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedbackService _feedbackService;
    public TechnicianFeedbackFilter(IUnitOfWork unitOfWork, IFeedbackService feedbackService)
    {
        _unitOfWork = unitOfWork;
        _feedbackService = feedbackService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get user ID from JWT
        var userId = context.HttpContext.User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Unauthorized"
            });
            return;
        }

        var userIdGuid = Guid.Parse(userId);

        // Get customer
        var technician = await _unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(c => c.Id == userIdGuid);

        if (technician == null)
        {
            context.Result = new NotFoundObjectResult(new
            {
                message = "Technician profile not found"
            });
            return;
        }

        var pendingBookingId = await _feedbackService.GetPendingTechnicianFeedbackBookingIdAsync(technician.Id);
        if (pendingBookingId.HasValue)
        {
            context.Result = new ObjectResult(new
            {
                StatusCode = 403,
                Error = "FeedbackPending",
                Message = "Please complete your feedback to be able to send new price offers.",
                Data = new
                {
                    PendingFeedbackBookingId = pendingBookingId,
                }
            })
            { StatusCode = 403 };

            return;
        }
        await next();
    }
}
