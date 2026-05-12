using Fixy.Domain.Entities;

namespace Fixy.Application.Contracts.Services;

public interface IBookingService
{
    Task CancelBookingByTechnicianAsync(ServiceBooking booking, Technician technician);
    Task CancelBookingByCustomerAsync(ServiceBooking booking, Guid customerId, bool reopenServiceRequest);
}
