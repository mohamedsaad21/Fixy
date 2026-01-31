using Fixy.Application.Abstracts;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Fixy.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;

    public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ServiceBooking> CreateOrUpdatePaymentIntentAsync(Guid BookingId)
    {
        StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];
        var customerId = _currentUserService.GetCurrentUserId();

        var booking = await _unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == BookingId);
        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found!");
        }

        var bookingAmount = (long)booking.AgreedPrice * 100;
        //
        decimal totalAmount = booking.AgreedPrice;
        decimal commissionRate = 0.15m; // example
        decimal platformCommission = totalAmount * commissionRate;
        decimal technicianAmount = totalAmount - platformCommission;


        var PaymentServices = new PaymentIntentService();

        if (booking.PaymentIntentId == null)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = bookingAmount,
                Currency = "USD",
                PaymentMethodTypes = ["card"],
            };
            var intent = await PaymentServices.CreateAsync(options);
            booking.PaymentIntentId = intent.Id;
            booking.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = bookingAmount,
            };
            await PaymentServices.UpdateAsync(booking.PaymentIntentId, options);
        }
        await _unitOfWork.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateBookingPaymentStatusAsync(string request, string stripeHeader)
    {
        var endPointSecret = _configuration["Stripe:EndPointSecret"];
        var stripeEvent = EventUtility.ConstructEvent(request, stripeHeader, endPointSecret);

        var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
        switch (stripeEvent.Type)
        {
            case EventTypes.PaymentIntentPaymentFailed:
                await UpdatePaymentFailedAsync(PaymentIntent.Id);
                break;
            case EventTypes.PaymentIntentSucceeded:
                await UpdatePaymentRecievedAsync(PaymentIntent.Id);
                break;
            default:
                Console.WriteLine($"Unhandled Stripe Event Types {stripeEvent.Type}");
                break;
        }
    }

    private async Task UpdatePaymentRecievedAsync(string paymentIntentId)
    {
        var booking = await _unitOfWork.Bookings.Find(x => x.PaymentIntentId == paymentIntentId);        
        booking.Status = ServiceBookingStatus.PaymentRecieved;
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdatePaymentFailedAsync(string paymentIntentId)
    {
        var booking = await _unitOfWork.Bookings.Find(x => x.PaymentIntentId == paymentIntentId);
        booking.Status = ServiceBookingStatus.PaymentFailed;
        await _unitOfWork.SaveChangesAsync();
    }
}
