namespace Fixy.Application.Features.Bookings.Queries.GetBookingById;

public record GetBookingByIdDto(Guid Id, string Status, decimal AgreedPrice, DateTime CreatedAt);
