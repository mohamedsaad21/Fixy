using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IStorageService fileService,
    IMapper mapper,
    ILogger<CreateServiceRequestCommandHandler> logger)
    : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var customerId = await currentUserService.GetCurrentUserId();

        logger.LogInformation("Customer creating service request. CustomerId: {CustomerId}, CategoryCount: {CategoryCount}, ImageCount: {ImageCount}", customerId, request.ServiceCategoriesIds.Count(), request.Images?.Count() ?? 0);

        var uploadedUrls = new List<string>();

        if (request.Images != null && request.Images.Any())
        {
            logger.LogInformation("Uploading service request images. CustomerId: {CustomerId}, ImageCount: {ImageCount}", customerId, request.Images.Count());

            foreach (var image in request.Images)
            {
                var url = await fileService.UploadAsync(image);
                uploadedUrls.Add(url);
            }

            logger.LogInformation("Service request images uploaded successfully. CustomerId: {CustomerId}, UploadedCount: {UploadedCount}", customerId, uploadedUrls.Count);
        }

        try
        {
            var serviceRequest = mapper.Map<ServiceRequest>(request);
            serviceRequest.CustomerId = customerId;

            var categories = new List<ServiceCategory>();
            foreach (var categoryId in request.ServiceCategoriesIds)
            {
                var category = await unitOfWork.ServiceCategories.GetByIdAsync(categoryId);
                if (category == null)
                {
                    logger.LogWarning("Service request creation failed — category not found. CustomerId: {CustomerId}, CategoryId: {CategoryId}", customerId, categoryId);

                    foreach (var url in uploadedUrls)
                        await fileService.DeleteAsync(url);

                    return Errors.ServiceCategoryNotFound;
                }
                categories.Add(category);
            }

            serviceRequest.ServiceCategories = categories;

            await unitOfWork.ServiceRequests.AddAsync(serviceRequest);

            foreach (var url in uploadedUrls)
            {
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = url,
                });
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Service request created successfully. ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}, CategoryCount: {CategoryCount}, ImageCount: {ImageCount}, ScheduledDateTime: {ScheduledDateTime}", serviceRequest.Id, customerId, categories.Count, uploadedUrls.Count, serviceRequest.ScheduledDateTime);

            return serviceRequest.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Service request creation failed — exception during processing. CustomerId: {CustomerId}, UploadedBeforeFailure: {UploadedBeforeFailure}", customerId, uploadedUrls.Count);

            foreach (var url in uploadedUrls)
                await fileService.DeleteAsync(url);

            logger.LogInformation("Rolled back uploaded images after failed service request creation. CustomerId: {CustomerId}, DeletedCount: {DeletedCount}", customerId, uploadedUrls.Count);

            return Errors.RequestInsertionFailed;
        }
    }
}