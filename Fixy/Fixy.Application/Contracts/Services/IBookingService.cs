using Fixy.Domain.Entities;
using Fixy.Domain.Enums;

namespace Fixy.Application.Contracts.Services;

public interface IBookingService
{
    Task CancelBookingAsync(ServiceBooking booking, Guid cancelledById);
}
