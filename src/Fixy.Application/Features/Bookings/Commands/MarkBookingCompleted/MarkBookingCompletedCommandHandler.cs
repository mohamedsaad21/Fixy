using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStorageService fileService, INotificationService notificationService) : IRequestHandler<MarkBookingCompletedCommand, Result>
{
    public async Task<Result> Handle(MarkBookingCompletedCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.InProgress)
            return Errors.BookingNotActive;

        foreach(var image in request.CompletionImages)
        {
            var url = await fileService.UploadAsync(image);
            await unitOfWork.ServiceBookingImages.AddAsync(new ServiceBookingImage
            {
                ImageUrl = url,
                ServiceBookingId = booking.Id
            });
        }

        booking.Status = ServiceBookingStatus.AwaitingCustomerConfirmationForCompletion;
        booking.CompletedAt = DateTime.UtcNow;

        var customer = booking.ServiceRequest.Customer;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.TechnicianCompleted,
            SharedResourcesKeys.NotificationTechnicianCompletedTitle,
            SharedResourcesKeys.NotificationTechnicianCompletedBody
        );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
