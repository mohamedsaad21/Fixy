using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;

public sealed class AddServiceRequestImagesCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService, ILogger<AddServiceRequestImagesCommandHandler> logger) : IRequestHandler<AddServiceRequestImagesCommand, Result>
{
    public async Task<Result> Handle(AddServiceRequestImagesCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding images to service request. ServiceRequestId: {ServiceRequestId}, ImageCount: {ImageCount}", request.ServiceRequestId, request.Images.Count());

        var serviceRequest = await unitOfWork.ServiceRequests.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);

        if (serviceRequest == null)
        {
            logger.LogWarning("Service request images upload failed — service request not found. ServiceRequestId: {ServiceRequestId}", request.ServiceRequestId);
            return Errors.ServiceRequestNotFound;
        }

        // Upload service request images
        var UploadResults = new List<string>();
        try {
            foreach (var image in request.Images)
            {
                var url = await fileService.UploadAsync(image);
                UploadResults.Add(url);
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = url,
                });
            }
            logger.LogInformation("All images uploaded successfully. ServiceRequestId: {ServiceRequestId}, UploadedCount: {UploadedCount}", request.ServiceRequestId, UploadResults.Count);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Service request images saved successfully. ServiceRequestId: {ServiceRequestId}, UploadedCount: {UploadedCount}", request.ServiceRequestId, UploadResults.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Service request images upload failed — exception during upload. ServiceRequestId: {ServiceRequestId}, UploadedBeforeFailure: {UploadedBeforeFailure}", request.ServiceRequestId, UploadResults.Count);
            foreach (var url in UploadResults)
            {
                await fileService.DeleteAsync(url);
            }
            logger.LogInformation("Rolled back uploaded images after failure. ServiceRequestId: {ServiceRequestId}, DeletedCount: {DeletedCount}", request.ServiceRequestId, UploadResults.Count);
            return Errors.RequestInsertionFailed;
        }
    }
}
