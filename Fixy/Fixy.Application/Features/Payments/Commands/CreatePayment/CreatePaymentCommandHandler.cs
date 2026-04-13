using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private const decimal PLATFORM_COMMISSION_RATE = 0.15m;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

    public async Task<Result<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Log.Information($"Creating payment for booking {request.BookingId}");

            // 1. Get booking details
            var booking = await _unitOfWork.Bookings.GetTableAsTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                Log.Warning($"Booking {request.BookingId} not found");
                return Errors.BookingNotFound;
            }

            // 2. Validate booking status
            if (booking.Status != ServiceBookingStatus.PaymentPending)
            {
                Log.Warning($"Booking {request.BookingId} is not in WaitingPayment status");
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
                // Create stripe payment URL
                var paymentUrlResult = await _paymentService.CreatePaymentUrlAsync(
                    totalAmount,
                    request.BookingId,
                    request.CustomerName,
                    request.CustomerEmail,
                    request.CustomerPhone
                );
                response.PaymentUrl = paymentUrlResult.PaymentUrl;
                payment.MerchantOrderId = paymentUrlResult.MerchantOrderId;

            }
            else if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Cash)
            {
                response.PaymentUrl = null;
            }

            // 7. Save payment
            await _unitOfWork.Payments.AddAsync(payment);
            response.PaymentId = payment.Id;

            // 8. Update booking status
            booking.Status = ServiceBookingStatus.PaymentPending;

            await _unitOfWork.SaveChangesAsync();

            // 9. Send notifications
            //await SendNotificationsAsync(request.PaymentMethod, booking, payment, cancellationToken);

            Log.Information($"Payment {payment.Id} created successfully for booking {request.BookingId}");

            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error creating payment for booking {request.BookingId}");
            return Errors.PaymentCreationFailed;
        }
    }
}