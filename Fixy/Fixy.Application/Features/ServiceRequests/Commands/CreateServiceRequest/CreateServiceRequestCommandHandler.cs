using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Common.Models;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CreateServiceRequestCommandHandler(IServiceRequestRepository serviceRequestRepository, ICurrentUserService currentUserService, IServiceCategoryRepository serviceCategoryRepository, IFileService fileService, IMapper mapper)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _currentUserService = currentUserService;
        _serviceCategoryRepository = serviceCategoryRepository;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        // Upload service request images
        var UploadResults = new List<UploadResultModel>();
        if (request.Images != null && request.Images.Any())
        {
            foreach (var image in request.Images)
            {
                var uploadResult = await _fileService.UploadAsync("ServiceRequest/temp", image);
                UploadResults.Add(uploadResult);
            }
        }
        await _serviceRequestRepository.BeginTransactionAsync();
        try
        {
            var serviceRequest = _mapper.Map<ServiceRequest>(request);
            // Assign customerId
            serviceRequest.CustomerId = _currentUserService.GetCurrentUserId();
            // Assign service categories
            var categories = new List<ServiceCategory>();
            foreach (var categoryId in request.ServiceCategoriesIds)
            {
                var category = await _serviceCategoryRepository.GetByIdAsync(categoryId);
                categories.Add(category);
            }
            serviceRequest.ServiceCategories = categories;
            // Add Service Request
            await _serviceRequestRepository.AddAsync(serviceRequest);
            foreach (var uploadResult in UploadResults)
            {
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = uploadResult.Url,
                    ImagePublicId = uploadResult.PublicId
                });
            }
            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _serviceRequestRepository.CommitAsync();
            return serviceRequest.Id;
        }
        catch (Exception)
        {
            foreach(var uploadResult in UploadResults)
            {
                await _fileService.DeleteAsync(uploadResult.PublicId);
            }
            await _serviceRequestRepository.RollBackAsync();
            return Errors.RequestInsertionFailed;
        }
    }
}
