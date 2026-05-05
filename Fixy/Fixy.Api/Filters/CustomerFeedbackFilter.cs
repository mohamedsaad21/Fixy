using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Api.Filters;

public class CustomerFeedbackFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedbackService _feedbackService;
    public CustomerFeedbackFilter(IUnitOfWork unitOfWork, IFeedbackService feedbackService)
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
        var customer = await _unitOfWork.Customers.GetTableNoTracking()
            .FirstOrDefaultAsync(c => c.Id == userIdGuid);

        if (customer == null)
        {
            context.Result = new NotFoundObjectResult(new
            {
                message = "Customer profile not found"
            });
            return;
        }

        var pendingBookingId = await _feedbackService.GetPendingCustomerFeedbackBookingIdAsync(customer.Id);
        if (pendingBookingId.HasValue)
        {
            context.Result = new ObjectResult(new
            {                
                StatusCode = 403,
                Error = "FeedbackPending",
                Message = "Please complete your feedback for your last booking first.",
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
