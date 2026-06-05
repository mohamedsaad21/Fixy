using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.PayCommission.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.PayCommission;

public class PayCommissionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
    IPaymentService stripeService, ILogger<PayCommissionCommandHandler> logger) : IRequestHandler<PayCommissionCommand, Result<PayCommissionResponse>>
{
    public async Task<Result<PayCommissionResponse>> Handle(PayCommissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get current user (technician)
            var technician = await currentUserService.GetCurrentUserAsync();

            // 2. Get unpaid commissions
            var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableNoTracking()
                .Where(c => c.TechnicianId == technician.Id && !c.IsPaid)
                .Include(c => c.Booking)
                .ToListAsync(cancellationToken);

            if (!commissions.Any())
            {
                logger.LogWarning("No unpaid commissions found for technician {TechnicianId}", technician.Id);
                return Errors.CommissionNoneFound;
            }

            // 3. Calculate total amount
            var totalAmount = commissions.Sum(c => c.AmountOwed);

            logger.LogInformation("Total commission amount: {Amount:C} for {Count} commissions",
                totalAmount, commissions.Count);

            // 4. Create Stripe PaymentIntent → returns ClientSecret for Angular Elements
            var paymentResult = await stripeService.CreatePaymentUrlAsync(
                amount: totalAmount,
                referenceId: technician.Id,
                customerName: request.TechnicianName,
                customerEmail: request.TechnicianEmail,
                customerPhone: request.TechnicianPhone,
                orderPrefix: "COMM"
            );

            if (!paymentResult.Success)
            {
                logger.LogError("Failed to create Stripe PaymentIntent for technician {TechnicianId}: {Error}", technician.Id, paymentResult.ErrorMessage);
                return Errors.PaymentCreationFailed;
            }

            // 5. Create Payment record to track this commission payment
            var commissionPayment = new Payment
            {
                UserId = technician.Id,
                TotalAmount = totalAmount,
                TechnicianAmount = 0,                         // Commission paid TO platform
                PlatformCommission = totalAmount,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Pending,
                MerchantOrderId = paymentResult.MerchantOrderId,
                PaymentIntentId = paymentResult.PaymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Payments.AddAsync(commissionPayment);

            // 6. Log included commissions (marked as paid in callback after success)
            foreach (var commission in commissions)
            {
                logger.LogInformation("Commission {CommissionId} included in payment {OrderRef}", commission.Id, paymentResult.MerchantOrderId);
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Commission payment initiated — Order: {OrderRef}, Amount: {Amount:C}",
                paymentResult.MerchantOrderId, totalAmount);

            // 7. Return ClientSecret to Angular — no redirect URL needed
            return new PayCommissionResponse
            {
                ClientSecret = paymentResult.ClientSecret,
                PaymentIntentId = paymentResult.PaymentIntentId,
                OrderReference = paymentResult.MerchantOrderId,
                TotalAmount = totalAmount,
                CommissionCount = commissions.Count,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating commission payment for technician");
            return Errors.PaymentCreationFailed;
        }
    }
}