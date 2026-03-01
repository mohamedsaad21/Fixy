using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.PayCommission.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.PayCommission;

public class PayCommissionCommandHandler
        : IRequestHandler<PayCommissionCommand, Result<PayCommissionResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPaymobService _paymobService;
    private readonly ILogger<PayCommissionCommandHandler> _logger;

    public PayCommissionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IPaymobService paymobService,
        ILogger<PayCommissionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _paymobService = paymobService;
        _logger = logger;
    }

    public async Task<Result<PayCommissionResponse>> Handle(PayCommissionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user (technician)
        var technician = await _currentUserService.GetCurrentUserAsync();

        // 2. Get  commissions
        var commissions = await _unitOfWork.TechnicianCommissionsOwed.GetTableNoTracking()
            .Where(c => c.TechnicianId == technician.Id && !c.IsPaid)
            .Include(c => c.Booking)
            .ToListAsync(cancellationToken);

        if (!commissions.Any())
        {
            _logger.LogWarning($"No unpaid commissions found for technician {technician.Id}");
            return Errors.CommissionNoneFound;
        }

        // 3. Calculate total amount
        var totalAmount = commissions.Sum(c => c.AmountOwed);

        _logger.LogInformation($"Total commission amount: {totalAmount:C} for {commissions.Count} commissions");

        // 5. Create Paymob payment URL for commission
        var paymentUrlResult = await _paymobService.CreatePaymentUrlAsync(
            totalAmount,
            technician.Id,
            request.TechnicianName,
            request.TechnicianEmail,
            request.TechnicianPhone,
            "COMM"
        );

        // 6. Create a Payment record to track this commission payment
        var commissionPayment = new Payment
        {
            UserId = technician.Id,  // Technician is the payer in this case
            TotalAmount = totalAmount,
            TechnicianAmount = 0,  // No technician earning, this is payment TO platform
            PlatformCommission = totalAmount,  // Full amount goes to platform
            PaymobOrderId = paymentUrlResult.PaymobOrderId.ToString(),
            Method = PaymentMethod.Card,
            Status = PaymentStatus.Pending,
            MerchantOrderId = paymentUrlResult.MerchantOrderId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(commissionPayment);

        // 7. Store commission IDs in a tracking table or use a different approach
        // For now, we'll store the merchant order ID in each commission for reference
        foreach (var commission in commissions)
        {
            // We'll mark them as paid in the callback after successful payment
            _logger.LogInformation($"Commission {commission.Id} included in payment {paymentUrlResult.MerchantOrderId}");
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"✅ Commission payment initiated - Merchant Order: {paymentUrlResult.MerchantOrderId}, Amount: {totalAmount:C}");

        // 8. Build response
        var response = new PayCommissionResponse
        {
            PaymentUrl = paymentUrlResult.PaymentUrl,
            TotalAmount = totalAmount,
            CommissionCount = commissions.Count,
            MerchantOrderId = paymentUrlResult.MerchantOrderId
        };

        return response;
    }
}
