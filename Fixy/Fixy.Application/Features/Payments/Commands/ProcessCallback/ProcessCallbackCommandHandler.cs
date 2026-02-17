using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public class ProcessCallbackCommandHandler : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymobService _paymobService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ProcessCallbackCommandHandler> _logger;

    public ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, IPaymobService paymobService,
        INotificationService notificationService, ILogger<ProcessCallbackCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _paymobService = paymobService;
        _notificationService = notificationService;
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

            // Verify HMAC
            //if (!_paymobService.VerifyHmac(query, hmac))
            //{
            //     return Errors.InvalidHmacSignature;
            //}

            // Find payment by MerchantOrderId
            var payment = await _unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId, cancellationToken);

            if (payment == null)
                return Errors.PaymentNotFound;


            // Check if already processed
            if (payment.Status == PaymentStatus.Success && !string.IsNullOrEmpty(payment.PaymobTransactionId))
                return true;

            // Update payment
            payment.PaymobTransactionId = transactionId;
            payment.PaymobOrderId = query["order"].ToString();
            payment.Status = success ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.PaidAt = success ? DateTime.UtcNow : null;

            // Update booking if successful
            if (success)
            {
                var booking = await _unitOfWork.Bookings.GetTableAsTracking()
                    .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId, cancellationToken);

                if (booking != null)
                {
                    booking.Status = ServiceBookingStatus.Completed;
                }
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync();

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing callback");
            return Errors.CallbackProcessingFailed;
        }
    }
}