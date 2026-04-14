using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService) : IRequestHandler<MarkBookingCompletedCommand, Result>
{
    public async Task<Result> Handle(MarkBookingCompletedCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.InProgress)
            return Errors.BookingNotActive;

        foreach(var image in request.CompletionImages)
        {
            var UploadResult = await fileService.UploadAsync($"Bookings/{booking.Id}", image);
            await unitOfWork.ServiceBookingImages.AddAsync(new ServiceBookingImage
            {
                ImageUrl = UploadResult.Url,
                ImagePublicId = UploadResult.PublicId,
                ServiceBookingId = booking.Id
            });
        }

        booking.Status = ServiceBookingStatus.Completed;
        booking.CompletedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
