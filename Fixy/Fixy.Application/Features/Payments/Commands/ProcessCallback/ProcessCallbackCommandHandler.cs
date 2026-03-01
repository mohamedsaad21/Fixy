using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed class ProcessCallbackCommandHandler : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymobService _paymobService;
    private readonly ILogger<ProcessCallbackCommandHandler> _logger;

    public ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, IPaymobService paymobService, ILogger<ProcessCallbackCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _paymobService = paymobService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var query = request.QueryData;

            // Extract data from query string
            var transactionId = query["id"].ToString();
            var success = query["success"].ToString().ToLower() == "true";
            var amountCents = query["amount_cents"].ToString();
            var merchantOrderId = query["merchant_order_id"].ToString();
            var hmac = query["hmac"].ToString();

            _logger.LogInformation($"Processing callback - Order: {merchantOrderId}, Success: {success}");

            // Verify HMAC (enable in production)
            //if (!_paymobService.VerifyHmac(query, hmac))
            //{
            //    _logger.LogError($"Invalid HMAC signature for order: {merchantOrderId}");
            //    return Errors.InvalidHmacSignature;
            //}

            // Find payment by MerchantOrderId
            var payment = await _unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId, cancellationToken);

            if (payment == null)
            {
                _logger.LogWarning($"Payment not found for order: {merchantOrderId}");
                return Errors.PaymentNotFound;
            }

            // Check if already processed
            if (payment.Status == PaymentStatus.Success && !string.IsNullOrEmpty(payment.PaymobTransactionId))
            {
                _logger.LogInformation($"Payment already processed: {merchantOrderId}");
                return true;
            }

            // Update payment
            payment.PaymobTransactionId = transactionId;
            payment.PaymobOrderId = query["order"].ToString();
            payment.Status = success ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.PaidAt = success ? DateTime.UtcNow : null;

            // Handle based on payment type
            if (success)
            {
                if (merchantOrderId.StartsWith("BK-"))
                {
                    // BOOKING PAYMENT
                    await HandleBookingPaymentSuccessAsync(payment, cancellationToken);
                }
                else if (merchantOrderId.StartsWith("COMM-"))
                {
                    // COMMISSION PAYMENT
                    await HandleCommissionPaymentSuccessAsync(payment, cancellationToken);
                }
                else
                {
                    _logger.LogWarning($"Unknown payment type: {merchantOrderId}");
                }
            }
            else
            {
                _logger.LogWarning($"Payment failed: {merchantOrderId}");
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Callback processed successfully: {merchantOrderId}");

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing callback");
            return Errors.CallbackProcessingFailed;
        }
    }

    // BOOKING PAYMENT SUCCESS
    private async Task HandleBookingPaymentSuccessAsync(Payment payment, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing successful booking payment: {payment.MerchantOrderId}");

        // Check if payment has a booking
        if (!payment.ServiceBookingId.HasValue)
        {
            _logger.LogWarning($"Booking payment has no ServiceBookingId: {payment.Id}");
            return;
        }

        // Get booking
        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning($"Booking not found: {payment.ServiceBookingId}");
            return;
        }

        // Update booking status
        booking.Status = ServiceBookingStatus.Completed;

        _logger.LogInformation($"Booking {booking.Id} marked as Paid");

        // Create payout for technician
        var payout = new Payout
        {
            TechnicianId = booking.TechnicianId,
            BookingId = booking.Id,
            Amount = payment.TechnicianAmount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payouts.AddAsync(payout);

        _logger.LogInformation($"Payout created: {payment.TechnicianAmount:C} for technician {booking.TechnicianId}");
    }

    // COMMISSION PAYMENT SUCCESS
    private async Task HandleCommissionPaymentSuccessAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing successful commission payment: {payment.MerchantOrderId}");

        // Extract technician ID from merchant order (COMM-{technicianId}-{random})
        var parts = payment.MerchantOrderId.Split('-');
        if (parts.Length < 2 || !Guid.TryParse(parts[1] + parts[2] + parts[3] + parts[4] + parts[5], out var technicianId))
        {
            _logger.LogError($"Cannot extract technician ID from: {payment.MerchantOrderId}");
            return;
        }

        // Get all unpaid commissions for this technician
        var commissions = await _unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
            .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
            .ToListAsync(cancellationToken);

        if (!commissions.Any())
        {
            _logger.LogWarning($"No unpaid commissions found for technician {technicianId}");
            return;
        }

        // Mark all as paid
        foreach (var commission in commissions)
        {
            commission.IsPaid = true;
            commission.PaidAt = DateTime.UtcNow;
        }

        _logger.LogInformation($"Marked {commissions.Count} commissions as paid for technician {technicianId}");
    }
}