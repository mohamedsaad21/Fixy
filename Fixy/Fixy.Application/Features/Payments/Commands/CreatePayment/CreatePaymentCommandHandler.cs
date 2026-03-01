using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymobService _paymobService;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;
    private const decimal PLATFORM_COMMISSION_RATE = 0.15m;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymobService paymobService, ILogger<CreatePaymentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _paymobService = paymobService;
        _logger = logger;
    }

    public async Task<Result<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Creating payment for booking {request.BookingId}");

            // 1. Get booking details
            var booking = await _unitOfWork.Bookings.GetTableAsTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                _logger.LogWarning($"Booking {request.BookingId} not found");
                return Errors.BookingNotFound;
            }

            // 2. Validate booking status
            if (booking.Status != ServiceBookingStatus.PaymentPending)
            {
                _logger.LogWarning($"Booking {request.BookingId} is not in WaitingPayment status");
                return Errors.BookingNotReadyForPayment;
            }
            // 3. Check if payment already exists
            var existingPayment = await _unitOfWork.Payments.GetTableNoTracking()
                .FirstOrDefaultAsync(p => p.ServiceBookingId == request.BookingId
                                       && p.Status == PaymentStatus.Success,
                                     cancellationToken);

            if (existingPayment != null)
                return Errors.PaymentAlreadyCompleted;

            // 4. Calculate amounts
            var totalAmount = booking.AgreedPrice;
            var platformCommission = totalAmount * PLATFORM_COMMISSION_RATE;
            var technicianAmount = totalAmount - platformCommission;

            // 5. Create payment record
            var payment = new Payment
            {
                ServiceBookingId = request.BookingId,
                UserId = request.CustomerId,
                TotalAmount = totalAmount,
                TechnicianAmount = totalAmount * 0.85m,
                PlatformCommission = totalAmount * 0.15m,
                //PaymobOrderId = paymentUrlResult.PaymobOrderId.ToString(),
                Method = (PaymentMethod)request.PaymentMethod,
                Status = PaymentStatus.Pending,
            };

            // 6. Handle based on payment method
            var response = new CreatePaymentResponse
            {
                PaymentMethod = (PaymentMethod)request.PaymentMethod,
                Amount = totalAmount,
            };

            if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Card)
            {
                // Create Paymob payment URL
                var paymentUrlResult = await _paymobService.CreatePaymentUrlAsync(
                    totalAmount,
                    request.BookingId,
                    request.CustomerName,
                    request.CustomerEmail,
                    request.CustomerPhone
                );

                payment.PaymobOrderId = paymentUrlResult.PaymobOrderId.ToString();
                payment.MerchantOrderId = paymentUrlResult.MerchantOrderId;
                response.PaymentUrl = paymentUrlResult.PaymentUrl;

                response.PaymentId = payment.Id;
                _logger.LogInformation($"Card payment created - Paymob Order: {payment.PaymobOrderId}");
            }
            else if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Cash)
            {
                response.PaymentUrl = null;
            }

            // 7. Save payment
            await _unitOfWork.Payments.AddAsync(payment);

            // 8. Update booking status
            booking.Status = ServiceBookingStatus.PaymentPending;

            await _unitOfWork.SaveChangesAsync();

            // 9. Send notifications
            //await SendNotificationsAsync(request.PaymentMethod, booking, payment, cancellationToken);

            _logger.LogInformation($"✅ Payment {payment.Id} created successfully for booking {request.BookingId}");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating payment for booking {request.BookingId}");
            return Errors.PaymentCreationFailed;
        }
    }
}