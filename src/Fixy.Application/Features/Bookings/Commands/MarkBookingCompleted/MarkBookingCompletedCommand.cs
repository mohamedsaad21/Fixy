using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public sealed record MarkBookingCompletedCommand
    (
        Guid BookingId,
        string CompletionNotes,
        List<IFormFile> CompletionImages
    ) : IRequest<Result>;
