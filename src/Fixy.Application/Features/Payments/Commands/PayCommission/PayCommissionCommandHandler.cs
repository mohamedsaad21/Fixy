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

public class PayCommissionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IPaymentService stripeService,
    ILogger<PayCommissionCommandHandler> logger) : IRequestHandler<PayCommissionCommand, Result<PayCommissionResponse>>
{
    public async Task<Result<PayCommissionResponse>> Handle(PayCommissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var technician = await currentUserService.GetCurrentUserAsync();

            // 1. Get unpaid commissions
            var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableNoTracking()
                .Where(c => c.TechnicianId == technician.Id && !c.IsPaid)
                .Include(c => c.Booking)
                .ToListAsync(cancellationToken);

            if (!commissions.Any())
            {
                logger.LogWarning("No unpaid commissions found for technician {TechnicianId}", technician.Id);
                return Errors.CommissionNoneFound;
            }

            var totalAmount = commissions.Sum(c => c.AmountOwed);

            logger.LogInformation("Total commission amount: {Amount:C} for {Count} commissions",
                totalAmount, commissions.Count);

            // 2. Check for existing pending commission payment
            var existingPayment = await unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.UserId == technician.Id
                                       && p.MerchantOrderId.StartsWith("COMM-")
                                       && p.Status == PaymentStatus.Pending,
                                     cancellationToken);

            if (existingPayment != null)
            {
                logger.LogInformation("Resuming existing pending commission payment for technician {TechnicianId}", technician.Id);

                var existingClientSecret = await GetExistingClientSecretAsync(existingPayment.PaymentIntentId);

                if (existingClientSecret != null)
                {
                    return new PayCommissionResponse
                    {
                        ClientSecret = existingClientSecret,
                        PaymentIntentId = existingPayment.PaymentIntentId,
                        OrderReference = existingPayment.MerchantOrderId,
                        TotalAmount = existingPayment.TotalAmount,
                        CommissionCount = commissions.Count,
                    };
                }

                // ✅ ClientSecret retrieval failed — mark old as Failed, create new
                logger.LogWarning("Could not retrieve ClientSecret for technician {TechnicianId}, creating new", technician.Id);
                existingPayment.Status = PaymentStatus.Failed;
                await unitOfWork.SaveChangesAsync();
            }

            // 3. Create new Stripe PaymentIntent
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
                logger.LogError("Failed to create Stripe PaymentIntent for technician {TechnicianId}: {Error}",
                    technician.Id, paymentResult.ErrorMessage);
                return Errors.PaymentCreationFailed;
            }

            // 4. Create new payment record
            var commissionPayment = new Payment
            {
                UserId = technician.Id,
                TotalAmount = totalAmount,
                TechnicianAmount = 0,
                PlatformCommission = totalAmount,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Pending,
                MerchantOrderId = paymentResult.MerchantOrderId,
                PaymentIntentId = paymentResult.PaymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Payments.AddAsync(commissionPayment);

            foreach (var commission in commissions)
                logger.LogInformation("Commission {CommissionId} included in payment {OrderRef}",
                    commission.Id, paymentResult.MerchantOrderId);

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Commission payment initiated — Order: {OrderRef}, Amount: {Amount:C}",
                paymentResult.MerchantOrderId, totalAmount);

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

    private async Task<string?> GetExistingClientSecretAsync(string? paymentIntentId)
    {
        if (string.IsNullOrEmpty(paymentIntentId))
            return null;

        try
        {
            var service = new Stripe.PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);
            return paymentIntent.ClientSecret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve existing PaymentIntent: {Id}", paymentIntentId);
            return null;
        }
    }
}