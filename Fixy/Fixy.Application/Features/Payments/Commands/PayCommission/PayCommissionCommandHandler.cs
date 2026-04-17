using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.PayCommission.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.PayCommission;

public class PayCommissionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IPaymentService paymobService) : IRequestHandler<PayCommissionCommand, Result<PayCommissionResponse>>
{
    public async Task<Result<PayCommissionResponse>> Handle(PayCommissionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //var currentTechnician = await currentUserService.GetCurrentUserAsync();

        //var wallet = await unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == currentTechnician.Id);

        //if (wallet == null)
        //    return Errors.WalletNotFound;



        //// Get current user (technician)
        //var technician = await currentUserService.GetCurrentUserAsync();

        //// Get  commissions
        //var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableNoTracking()
        //    .Where(c => c.TechnicianId == technician.Id && !c.IsPaid)
        //    .Include(c => c.Booking)
        //    .ToListAsync(cancellationToken);

        //if (!commissions.Any())
        //{
        //    Log.Warning($"No unpaid commissions found for technician {technician.Id}");
        //    return Errors.CommissionNoneFound;
        //}

        //// Calculate total amount
        //var totalAmount = commissions.Sum(c => c.AmountOwed);

        //Log.Information($"Total commission amount: {totalAmount:C} for {commissions.Count} commissions");

        //// Create payment URL for commission
        //var paymentUrlResult = await paymobService.CreatePaymentUrlAsync(
        //    totalAmount,
        //    technician.Id,
        //    request.TechnicianName,
        //    request.TechnicianEmail,
        //    request.TechnicianPhone,
        //    "COMM"
        //);

        //// Create a Payment record to track this commission payment
        //var commissionPayment = new Payment
        //{
        //    UserId = technician.Id,
        //    TotalAmount = totalAmount,
        //    TechnicianAmount = 0,
        //    PlatformCommission = totalAmount,
        //    //PaymobOrderId = paymentUrlResult.PaymobOrderId.ToString(),
        //    Method = PaymentMethod.Card,
        //    Status = PaymentStatus.Pending,
        //    MerchantOrderId = paymentUrlResult.MerchantOrderId,
        //    CreatedAt = DateTime.UtcNow
        //};

        //await unitOfWork.Payments.AddAsync(commissionPayment);

        //// 7. Store commission IDs in a tracking table or use a different approach
        //// For now, we'll store the merchant order ID in each commission for reference
        //foreach (var commission in commissions)
        //{
        //    // We'll mark them as paid in the callback after successful payment
        //    Log.Information($"Commission {commission.Id} included in payment {paymentUrlResult.MerchantOrderId}");
        //}

        //await unitOfWork.SaveChangesAsync();

        //Log.Information($"Commission payment initiated - Merchant Order: {paymentUrlResult.MerchantOrderId}, Amount: {totalAmount:C}");

        //// 8. Build response
        //var response = new PayCommissionResponse
        //{
        //    PaymentUrl = paymentUrlResult.PaymentUrl,
        //    TotalAmount = totalAmount,
        //    CommissionCount = commissions.Count,
        //    MerchantOrderId = paymentUrlResult.MerchantOrderId
        //};

        //return response;
    }
}
