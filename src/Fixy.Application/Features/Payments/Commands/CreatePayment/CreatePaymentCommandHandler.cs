using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, ILogger<CreatePaymentCommandHandler> logger) : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
{
    private const decimal PLATFORM_COMMISSION_RATE = 0.10m;
    public async Task<Result<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating payment for booking {BookingId}", request.BookingId);

            // 1. Get booking details
            var booking = await unitOfWork.Bookings.GetTableAsTracking()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                logger.LogWarning("Booking {BookingId} not found", request.BookingId);
                return Errors.BookingNotFound;
            }

            // 2. Validate booking status
            if (booking.Status != ServiceBookingStatus.AwaitingPayment)
            {
                logger.LogWarning("Booking {BookingId} is not in AwaitingPayment status", request.BookingId);
                return Errors.BookingNotReadyForPayment;
            }

            // 3. Check if payment already exists
            var existingPayment = await unitOfWork.Payments.GetTableNoTracking()
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
                var paymentResult = await paymentService.CreatePaymentUrlAsync(
                    amount: totalAmount,
                    referenceId: request.BookingId,
                    customerName: request.CustomerName,
                    customerEmail: request.CustomerEmail,
                    customerPhone: request.CustomerPhone,
                    orderPrefix: "BK"
                );

                if (!paymentResult.Success)
                {
                    logger.LogError("Failed to create Stripe PaymentIntent for booking {BookingId}: {Error}",
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
            await unitOfWork.Payments.AddAsync(payment);
            response.PaymentId = payment.Id;

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Payment {PaymentId} created successfully for booking {BookingId}",
                payment.Id, request.BookingId);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payment for booking {BookingId}", request.BookingId);
            return Errors.PaymentCreationFailed;
        }
    }
}