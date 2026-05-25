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
            Log.Information("Creating payment for booking {BookingId}", request.BookingId);

            // 1. Get booking details
            var booking = await _unitOfWork.Bookings.GetTableAsTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                Log.Warning("Booking {BookingId} not found", request.BookingId);
                return Errors.BookingNotFound;
            }

            // 2. Validate booking status
            if (booking.Status != ServiceBookingStatus.AwaitingPayment)
            {
                Log.Warning("Booking {BookingId} is not in AwaitingPayment status", request.BookingId);
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
                TechnicianAmount = technicianAmount,
                PlatformCommission = platformCommission,
                Method = (PaymentMethod)request.PaymentMethod,
                Status = PaymentStatus.Pending,
            };

            // 6. Build response
            var response = new CreatePaymentResponse
            {
                PaymentMethod = (PaymentMethod)request.PaymentMethod,
                Amount = totalAmount,
            };

            // 7. Handle based on payment method
            if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Card)
            {
                // Create Stripe PaymentIntent → returns ClientSecret for Angular Elements
                var paymentResult = await _paymentService.CreatePaymentUrlAsync(
                    amount: totalAmount,
                    referenceId: request.BookingId,
                    customerName: request.CustomerName,
                    customerEmail: request.CustomerEmail,
                    customerPhone: request.CustomerPhone,
                    orderPrefix: "BK"
                );

                if (!paymentResult.Success)
                {
                    Log.Error("Failed to create Stripe PaymentIntent for booking {BookingId}: {Error}",
                        request.BookingId, paymentResult.ErrorMessage);
                    return Errors.PaymentCreationFailed;
                }

                // Store Stripe references on the payment record
                payment.MerchantOrderId = paymentResult.MerchantOrderId;
                payment.PaymentIntentId = paymentResult.PaymentIntentId;

                // Return ClientSecret to Angular — no redirect URL needed
                response.ClientSecret = paymentResult.ClientSecret;
                response.PaymentIntentId = paymentResult.PaymentIntentId;
                response.OrderReference = paymentResult.MerchantOrderId;
            }
            else if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Cash)
            {
                // Cash payments don't need Stripe — just assign a reference
                payment.MerchantOrderId = $"BK-{request.BookingId.ToString().ToUpper()}";
                response.ClientSecret = null;
                response.OrderReference = payment.MerchantOrderId;
            }

            // 8. Save payment
            await _unitOfWork.Payments.AddAsync(payment);
            response.PaymentId = payment.Id;

            await _unitOfWork.SaveChangesAsync();

            Log.Information("Payment {PaymentId} created successfully for booking {BookingId}",
                payment.Id, request.BookingId);

            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating payment for booking {BookingId}", request.BookingId);
            return Errors.PaymentCreationFailed;
        }
    }
}