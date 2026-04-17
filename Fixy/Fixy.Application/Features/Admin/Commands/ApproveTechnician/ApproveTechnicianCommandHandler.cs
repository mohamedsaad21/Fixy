using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, IEmailService emailService) : IRequestHandler<ApproveTechnicianCommand, Result>
{
    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.IsActive)
            return Errors.TechnicianAlreadyApproved;

        technician.IsActive = true;

        await unitOfWork.Wallets.AddAsync(new Wallet
        {
            ApplicationUserId = technician.Id,
            Balance = 0
        });

        var stripeAccountId = await paymentService.CreateConnectAccountAsync(
            technicianId: technician.Id.ToString(),
            email: technician.Email,
            technician.FirstName,
            technician.LastName);

        var onboardingUrl = await paymentService.CreateOnboardingLinkAsync(stripeAccountId);

        technician.StripeAccountId = stripeAccountId;
        technician.IsStripeOnboarded = false;  // true only after they complete
        technician.StripeOnboardingUrl = onboardingUrl;

        await unitOfWork.SaveChangesAsync();
        string message = $@"
        Dear {technician.FirstName},

        Congratulations! Your technician account on Fixy has been approved by our administration team. 
        
        To begin receiving payments for your services, you must complete your payment setup via Stripe. Please click the link below to enter your banking details and verify your account:
        
        {onboardingUrl}
        
        If you have any questions, feel free to contact our support team.";
        await emailService.SendEmailAsync(technician.Email, message, "Account Approval");
        return Result.Success();
    }
}
