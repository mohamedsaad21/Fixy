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
            var booking = await _unitOfWork.Bookings.GetTableNoTracking()
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

            var totalAmount = booking.AgreedPrice;

            // 3. Check if payment already exists
            var existingPayment = await _unitOfWork.Payments.GetTableNoTracking()
                .FirstOrDefaultAsync(p => p.ServiceBookingId == request.BookingId
                                       && p.Status == PaymentStatus.Success,
                                     cancellationToken);

            if (existingPayment != null)
            {
                _logger.LogWarning($"Payment already completed for booking {request.BookingId}");
                return Errors.PaymentAlreadyCompleted;
            }
            // 4. Create payment URL via Paymob
            var paymentUrlResult = await _paymobService.CreatePaymentUrlAsync(
                totalAmount,
                request.BookingId,
                request.CustomerName,
                request.CustomerEmail,
                request.CustomerPhone
            );

            // 5. Save payment record
            var payment = new Payment
            {
                ServiceBookingId = request.BookingId,
                CustomerId = request.CustomerId,
                //TechnicianId = booking.TechnicianId,
                TotalAmount = totalAmount,
                TechnicianAmount = totalAmount * 0.85m,
                PlatformCommission = totalAmount * 0.15m,
                PaymobOrderId = paymentUrlResult.PaymobOrderId.ToString(),
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Pending,
                MerchantOrderId = paymentUrlResult.MerchantOrderId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Payment {payment.Id} created successfully for booking {request.BookingId}");

            return new CreatePaymentResponse
            {
                PaymentId = payment.Id,
                PaymentUrl = paymentUrlResult.PaymentUrl,
                Amount = totalAmount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating payment for booking {request.BookingId}");
            return Errors.PaymentCreationFailed;
        }
    }
}