using Fixy.Domain.Constants;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Api.Filters;

public class TechnicianStatusFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    public TechnicianStatusFilter(IUnitOfWork unitOfWork)
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

        if (context.HttpContext.User.IsInRole(Roles.Admin))
        {
            await next();
            return;
        }

        var userIdGuid = Guid.Parse(userId);

        // Get technician
        var technician = await _unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(t => t.Id == userIdGuid);

        if (technician == null)
        {
            context.Result = new NotFoundObjectResult(new
            {
                message = "Technician profile not found"
            });
            return;
        }

        // Check technician status
        switch (technician.Status)
        {
            case TechnicianStatus.PendingApproval:
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Your account is pending approval by admin. You will be notified once approved.",
                    status = "PendingApproval",
                    canOperate = false
                })
                {
                    StatusCode = 403
                };
                return;

            case TechnicianStatus.Blocked:
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Your account has been blocked. Please contact support.",
                    reason = technician.BlockReason,
                    status = "Blocked",
                    canOperate = false
                })
                {
                    StatusCode = 403
                };
                return;

            case TechnicianStatus.Rejected:
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Your application has been rejected.",
                    reason = technician.RejectionReason,
                    status = "Rejected",
                    canOperate = false
                })
                {
                    StatusCode = 403
                };
                return;

            case TechnicianStatus.Approved:
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
