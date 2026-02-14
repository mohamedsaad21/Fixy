using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler : IRequestHandler<ApproveTechnicianCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;
    private readonly IStripeConnectService _stripeConnectService;

    public ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
        INotificationService notificationService, IStripeConnectService stripeConnectService)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
        _stripeConnectService = stripeConnectService;
    }

    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await _unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.IsActive)
            return Errors.TechnicianAlreadyApproved;

        technician.IsActive = true;

        //var stripeAccountId = await _stripeConnectService.CreateTechnicianAccountAsync(
        //        technician.Id,
        //        technician.Email,
        //        "US" // or get from technician profile
        //    );

        //// Generate onboarding link
        //var onboardingUrl = await _stripeConnectService.CreateOnboardingLinkAsync(
        //    stripeAccountId,
        //    returnUrl: $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/technician/stripe/success",
        //    refreshUrl: $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/technician/stripe/refresh"
        //);

        // Send email to technician with onboarding link
        await _unitOfWork.SaveChangesAsync();
        // Notify technician that account has been approved
        await _notificationService.NotifyAccountApprovedAsync(technician.Id);
        return Result.Success();
    }
}
