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

            var booking = await unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                logger.LogWarning("Booking {BookingId} not found", request.BookingId);
                return Errors.BookingNotFound;
            }

            if (booking.Status != ServiceBookingStatus.AwaitingPayment)
            {
                logger.LogWarning("Booking {BookingId} is not in AwaitingPayment status", request.BookingId);
                return Errors.BookingNotReadyForPayment;
            }

            var existingPayment = await unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.ServiceBookingId == request.BookingId
                                       && p.Status != PaymentStatus.Failed,
                                     cancellationToken);

            if (existingPayment != null)
            {
                if (existingPayment.Status == PaymentStatus.Success)
                    return Errors.PaymentAlreadyCompleted;

                if (existingPayment.Status == PaymentStatus.Pending)
                {
                    var requestedMethod = (PaymentMethod)request.PaymentMethod;

                    if (requestedMethod == PaymentMethod.Cash)
                    {
                        logger.LogInformation("Switching pending payment to Cash for booking {BookingId}", request.BookingId);
                        existingPayment.Method = PaymentMethod.Cash;
                        existingPayment.PaymentIntentId = null;
                        existingPayment.MerchantOrderId = $"BK-{request.BookingId.ToString().ToUpper()}";
                        await unitOfWork.SaveChangesAsync();
                        return new CreatePaymentResponse
                        {
                            PaymentId = existingPayment.Id,
                            PaymentMethod = PaymentMethod.Cash,
                            Amount = existingPayment.TotalAmount,
                            ClientSecret = null,
                            OrderReference = existingPayment.MerchantOrderId,
                        };
                    }

                    if (requestedMethod == PaymentMethod.Card
                        && existingPayment.Method == PaymentMethod.Card
                        && !string.IsNullOrEmpty(existingPayment.PaymentIntentId))
                    {
                        logger.LogInformation("Resuming existing pending card payment for booking {BookingId}", request.BookingId);

                        var clientSecret = await GetExistingClientSecretAsync(existingPayment.PaymentIntentId);

                        if (clientSecret != null)
                        {
                            return new CreatePaymentResponse
                            {
                                PaymentId = existingPayment.Id,
                                PaymentMethod = PaymentMethod.Card,
                                Amount = existingPayment.TotalAmount,
                                ClientSecret = clientSecret,
                                PaymentIntentId = existingPayment.PaymentIntentId,
                                OrderReference = existingPayment.MerchantOrderId,
                            };
                        }

                        logger.LogWarning("Could not retrieve ClientSecret, creating new PaymentIntent for booking {BookingId}", request.BookingId);
                    }

                    existingPayment.Status = PaymentStatus.Failed;
                    await unitOfWork.SaveChangesAsync();
                }
            }

            var totalAmount = booking.AgreedPrice;
            var platformCommission = totalAmount * PLATFORM_COMMISSION_RATE;
            var technicianAmount = totalAmount - platformCommission;

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

            var response = new CreatePaymentResponse
            {
                PaymentMethod = (PaymentMethod)request.PaymentMethod,
                Amount = totalAmount,
            };

            if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Card)
            {
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

                payment.MerchantOrderId = paymentResult.MerchantOrderId;
                payment.PaymentIntentId = paymentResult.PaymentIntentId;

                response.ClientSecret = paymentResult.ClientSecret;
                response.PaymentIntentId = paymentResult.PaymentIntentId;
                response.OrderReference = paymentResult.MerchantOrderId;
            }
            else if ((PaymentMethod)request.PaymentMethod == PaymentMethod.Cash)
            {
                payment.MerchantOrderId = $"BK-{request.BookingId.ToString().ToUpper()}";
                response.ClientSecret = null;
                response.OrderReference = payment.MerchantOrderId;
            }

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

    private async Task<string?> GetExistingClientSecretAsync(string? paymentIntentId)
    {
        if (string.IsNullOrEmpty(paymentIntentId))
            return null;

        try
        {
            var service = new Stripe.PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);
            return paymentIntent.ClientSecret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve existing PaymentIntent: {Id}", paymentIntentId);
            return null;
        }
    }
}