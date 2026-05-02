using Fixy.Domain.Entities;

namespace Fixy.Application.Contracts.Services;

public interface IFeedbackService
{
    Task ProcessFeedbackCompletionAsync(ServiceBooking Booking);
}
