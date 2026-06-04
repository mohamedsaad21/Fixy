using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStorageService _fileService;
    private readonly IMapper _mapper;

    public CreateServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, 
        IStorageService fileService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        // Upload service request images
        var UploadResults = new List<string>();
        if (request.Images != null && request.Images.Any())
        {
            foreach (var image in request.Images)
            {
                var uploadResult = await _fileService.UploadAsync(image);
                UploadResults.Add(uploadResult);
            }
        }
        try
        {
            var serviceRequest = _mapper.Map<ServiceRequest>(request);
            // Assign customerId
            serviceRequest.CustomerId = await _currentUserService.GetCurrentUserId();
            // Assign service categories
            var categories = new List<ServiceCategory>();
            foreach (var categoryId in request.ServiceCategoriesIds)
            {
                var category = await _unitOfWork.ServiceCategories.GetByIdAsync(categoryId);
                categories.Add(category);
            }
            serviceRequest.ServiceCategories = categories;
            // Add Service Request
            await _unitOfWork.ServiceRequests.AddAsync(serviceRequest);
            foreach (var url in UploadResults)
            {
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = url,
                });
            }
            await _unitOfWork.SaveChangesAsync();
            return serviceRequest.Id;
        }
        catch (Exception)
        {
            foreach(var url in UploadResults)
            {
                await _fileService.DeleteAsync(url);
            }
            return Errors.RequestInsertionFailed;
        }
    }
}
