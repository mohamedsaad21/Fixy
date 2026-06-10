using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStorageService fileService, ILogger<MarkBookingCompletedCommandHandler>logger) : IRequestHandler<MarkBookingCompletedCommand, Result>
{
    public async Task<Result> Handle(MarkBookingCompletedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician attempting to mark booking as completed. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Mark booking completed failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
        {
            logger.LogWarning("Mark booking completed failed — unauthorized technician. BookingId: {BookingId}, BookingTechnicianId: {BookingTechnicianId}, RequestingTechnicianId: {RequestingTechnicianId}", request.BookingId, booking.TechnicianId, currentTechnician.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.InProgress)
        {
            logger.LogWarning("Mark booking completed failed — booking is not active. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.BookingNotActive;
        }

        var imageCount = request.CompletionImages.Count();
        logger.LogInformation("Uploading completion evidence images. BookingId: {BookingId}, ImageCount: {ImageCount}", request.BookingId, imageCount);

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

        logger.LogInformation("Completion evidence images uploaded successfully. BookingId: {BookingId}, UploadedCount: {UploadedCount}", request.BookingId, uploadedUrls.Count);

        booking.Status = ServiceBookingStatus.AwaitingCustomerConfirmationForCompletion;
        booking.CompletedAt = DateTime.UtcNow;

        var customer = booking.ServiceRequest.Customer;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Booking marked as completed by technician — awaiting customer confirmation. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, EvidenceImageCount: {EvidenceImageCount}, CompletedAt: {CompletedAt}",
            request.BookingId, currentTechnician.Id, booking.ServiceRequest.Customer.Id,
            uploadedUrls.Count, booking.CompletedAt);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            customer.Id,
            NotificationType.TechnicianCompleted,
            SharedResourcesKeys.NotificationTechnicianCompletedTitle,
            SharedResourcesKeys.NotificationTechnicianCompletedBody,
            new Dictionary<string, string> { { "bookingId", booking.Id.ToString() } }
        ));

        return Result.Success();
    }
}