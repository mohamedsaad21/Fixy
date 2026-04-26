using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Api.Filters;

public class CustomerStatusFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    public CustomerStatusFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        // Check customer status
        switch (customer.Status)
        {
            case CustomerStatus.Blocked:
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Your account has been blocked. Please contact support for assistance.",
                    reason = customer.BlockReason,
                    status = "Blocked",
                    canOperate = false,
                    blockedAt = customer.BlockedAt
                })
                {
                    StatusCode = 403
                };
                return;

            case CustomerStatus.Active:
                await next();
                return;

            default:
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Invalid account status",
                    canOperate = false
                })
                {
                    StatusCode = 403
                };
                return;
        }
    }
}
