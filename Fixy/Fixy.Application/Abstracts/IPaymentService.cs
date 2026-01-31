using Fixy.Domain.Entities;

namespace Fixy.Application.Abstracts;

public interface IPaymentService
{
    Task<ServiceBooking> CreateOrUpdatePaymentIntentAsync(Guid BookingId);
    Task UpdateBookingPaymentStatusAsync(string request, string stripeHeader);
}
