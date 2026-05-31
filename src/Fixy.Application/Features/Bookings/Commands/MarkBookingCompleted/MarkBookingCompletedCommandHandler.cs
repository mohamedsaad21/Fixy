using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStorageService fileService, INotificationService notificationService) : IRequestHandler<MarkBookingCompletedCommand, Result>
{
    public async Task<Result> Handle(MarkBookingCompletedCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Technician attempting to mark booking as completed. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            Log.Warning("Mark booking completed failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
        {
            Log.Warning("Mark booking completed failed — unauthorized technician. BookingId: {BookingId}, BookingTechnicianId: {BookingTechnicianId}, RequestingTechnicianId: {RequestingTechnicianId}", request.BookingId, booking.TechnicianId, currentTechnician.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.InProgress)
        {
            Log.Warning("Mark booking completed failed — booking is not active. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.BookingNotActive;
        }

        var imageCount = request.CompletionImages.Count();
        Log.Information("Uploading completion evidence images. BookingId: {BookingId}, ImageCount: {ImageCount}", request.BookingId, imageCount);

        var uploadedUrls = new List<string>();

        foreach(var image in request.CompletionImages)
        {
            var url = await fileService.UploadAsync(image);
            uploadedUrls.Add(url);
            await unitOfWork.ServiceBookingImages.AddAsync(new ServiceBookingImage
            {
                ImageUrl = url,
                ServiceBookingId = booking.Id
            });
        }

        Log.Information("Completion evidence images uploaded successfully. BookingId: {BookingId}, UploadedCount: {UploadedCount}", request.BookingId, uploadedUrls.Count);

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
        Log.Information("Booking marked as completed by technician — awaiting customer confirmation. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, EvidenceImageCount: {EvidenceImageCount}, CompletedAt: {CompletedAt}",
            request.BookingId, currentTechnician.Id, booking.ServiceRequest.Customer.Id,
            uploadedUrls.Count, booking.CompletedAt);
        return Result.Success();
    }
}