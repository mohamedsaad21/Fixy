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
    private readonly INotificationService _notificationService;
    private readonly IPaymobService _paymobService;
    private readonly ILogger<ProcessCallbackCommandHandler> _logger;

    public ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService,
         IPaymobService paymobService, ILogger<ProcessCallbackCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing Paymob callback");

            //1.Verify HMAC signature
            //if (!_paymobService.VerifyHmacSignature(request.Callback))
            //{
            //    _logger.LogWarning("Invalid HMAC signature in Paymob callback");
            //    return Errors.InvalidHmacSignature;
            //}

            var transaction = request.Callback.obj;
            var merchantOrderId = transaction.order.merchant_Order_Id;

            // 2. Extract booking ID from merchant order ID
            if (!merchantOrderId.StartsWith("BK-"))
            {
                _logger.LogWarning($"Invalid merchant order ID format: {merchantOrderId}");
                return Errors.InvalidMerchantOrderId;
            }

            // 3. Find payment record
            var payment = await _unitOfWork.Payments.GetTableNoTracking()
                .Include(p => p.ServiceBooking)
                //.Include(p => p.Technician)
                //.Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId, cancellationToken);

            if (payment == null)
            {
                _logger.LogWarning($"Payment not found for booking {payment.ServiceBookingId}");
                return Errors.PaymentNotFound;
            }

            // 4. Update payment record
            payment.PaymobTransactionId = transaction.id.ToString();
            payment.PaymobOrderId = transaction.order.id.ToString();
            payment.Status = transaction.success ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.PaidAt = transaction.success ? DateTime.UtcNow : null;

            await _unitOfWork.Payments.UpdateAsync(payment);

            // 5. Update booking if payment successful
            if (transaction.success)
            {
                var booking = payment.ServiceBooking;
                booking.Status = ServiceBookingStatus.Completed;

                await _unitOfWork.Bookings.UpdateAsync(booking);

                _logger.LogInformation($"Payment successful for booking {payment.ServiceBookingId}. Amount: {payment.TotalAmount} EGP");

                //// 6. Send notifications
                //await _notificationService.NotifyPaymentReceivedAsync(
                //    payment.CustomerId,
                //    new
                //    {
                //        bookingId = booking.Id,
                //        amount = payment.TotalAmount,
                //        paymentMethod = "Card",
                //        message = "Payment completed successfully"
                //    }
                //);

                //await _notificationService.NotifyPaymentReceivedAsync(
                //    payment.TechnicianId,
                //    new
                //    {
                //        bookingId = booking.Id,
                //        amount = payment.TechnicianAmount,
                //        paymentMethod = "Card",
                //        message = $"Customer paid {payment.TotalAmount} EGP. Your share: {payment.TechnicianAmount} EGP"
                //    }
                //);
            }
            else
            {
                _logger.LogWarning($"Payment failed for booking {payment.ServiceBookingId}");
            }

            await _unitOfWork.SaveChangesAsync();

            return transaction.success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Paymob callback");
            return Errors.CallbackProcessingFailed;
        }
    }
}
