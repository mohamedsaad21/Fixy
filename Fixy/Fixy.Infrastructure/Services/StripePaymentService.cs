using Fixy.Application.Abstracts;
using Fixy.Application.Common.DTOs;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly StripeSettings _stripeSettings;

    public StripePaymentService(IUnitOfWork unitOfWork, StripeSettings stripeSettings)
    {
        _unitOfWork = unitOfWork;
        _paymentIntentService = new PaymentIntentService();
        _stripeSettings = stripeSettings;
    }

    /// <summary>
    /// Create payment intent for a booking (ESCROW STARTS HERE)
    /// Money goes to PLATFORM Stripe account and is HELD
    /// </summary>
    public async Task<PaymentIntentResultDto> CreatePaymentIntentAsync(Guid bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .Include(b => b.ServiceRequest).ThenInclude(s => s.Customer)
            .Include(b => b.Technician)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            throw new Exception("Booking not found");

        // Check if payment already exists
        var existingPayment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.ServiceBookingId == bookingId);

        if (existingPayment != null)
        {
            // Return existing payment intent
            var existingIntent = await _paymentIntentService.GetAsync(existingPayment.StripePaymentIntentId);
            return new PaymentIntentResultDto
            {
                PaymentIntentId = existingPayment.StripePaymentIntentId,
                ClientSecret = existingIntent.ClientSecret,
                Amount = existingPayment.Amount,
                PlatformCommission = existingPayment.PlatformCommission,
                TechnicianAmount = existingPayment.TechnicianAmount
            };
        }

        // Calculate amounts
        var totalAmount = booking.AgreedPrice;
        var platformCommission = totalAmount * (_stripeSettings.PlatformCommissionPercentage / 100);
        var technicianAmount = totalAmount - platformCommission;

        // Convert to cents (Stripe uses smallest currency unit)
        var amountInCents = (long)(totalAmount * 100);

        // Create PaymentIntent on PLATFORM account (not seller account)
        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = "usd",

            // CRITICAL: This is captured on YOUR platform account
            // Money is HELD by Stripe, assigned to your platform
            CaptureMethod = "automatic",

            // Enable automatic payment methods (cards, wallets, etc.)
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },

            // Metadata for tracking
            Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId.ToString() },
                    { "customer_id", booking.ServiceRequest.CustomerId.ToString() },
                    { "technician_id", booking.TechnicianId.ToString() },
                    { "platform_commission", platformCommission.ToString("F2") },
                    { "technician_amount", technicianAmount.ToString("F2") }
                },

            // Optional: attach to Stripe customer for easier refunds/tracking
            // Customer = stripeCustomerId,

            Description = $"Service booking #{bookingId}"
        };

        var paymentIntent = await _paymentIntentService.CreateAsync(options);

        // Store payment record in database
        var payment = new Domain.Entities.Payment
        {
            ServiceBookingId = bookingId,
            StripePaymentIntentId = paymentIntent.Id,
            Amount = totalAmount,
            PlatformCommission = platformCommission,
            TechnicianAmount = technicianAmount,
            Currency = "usd",
            Status = paymentIntent.Status, // "requires_payment_method" initially
        };

        await _unitOfWork.Payments.AddAsync(payment);

        // Update booking status
        booking.Status = ServiceBookingStatus.PaymentPending;

        await _unitOfWork.SaveChangesAsync();

        return new PaymentIntentResultDto
        {
            PaymentIntentId = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret,
            Amount = totalAmount,
            PlatformCommission = platformCommission,
            TechnicianAmount = technicianAmount
        };
    }

    /// <summary>
    /// Confirm payment (usually done by frontend with card details)
    /// </summary>
    public async Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId)
    {
        var paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntentId);

        // Update payment status
        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);

        if (payment != null)
        {
            payment.Status = paymentIntent.Status;
            if (paymentIntent.Status == "succeeded")
            {
                payment.PaidAt = DateTime.UtcNow;

                // Update booking
                var booking = await _unitOfWork.Bookings.Find(x => x.Id == payment.ServiceBookingId);
                if (booking != null)
                {
                    booking.Status = ServiceBookingStatus.Active; // Payment received, service can start
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        return paymentIntent;
    }

    /// <summary>
    /// Cancel payment before it's captured
    /// </summary>
    public async Task<bool> CancelPaymentAsync(string paymentIntentId)
    {
        try
        {
            var options = new PaymentIntentCancelOptions();
            await _paymentIntentService.CancelAsync(paymentIntentId, options);

            var payment = await _unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);

            if (payment != null)
            {
                payment.Status = "canceled";
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
