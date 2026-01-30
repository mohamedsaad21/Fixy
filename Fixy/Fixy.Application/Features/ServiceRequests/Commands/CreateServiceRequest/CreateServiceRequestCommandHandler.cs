using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Common.Models;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public CreateServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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
        try
        {
            var serviceRequest = _mapper.Map<ServiceRequest>(request);
            // Assign customerId
            serviceRequest.CustomerId = _currentUserService.GetCurrentUserId();
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
            foreach (var uploadResult in UploadResults)
            {
                serviceRequest.ServiceRequestImages.Add(new ServiceRequestImage
                {
                    ImageUrl = uploadResult.Url,
                    ImagePublicId = uploadResult.PublicId
                });
            }
            await _unitOfWork.SaveChangesAsync();
            return serviceRequest.Id;
        }
        catch (Exception)
        {
            foreach(var uploadResult in UploadResults)
            {
                await _fileService.DeleteAsync(uploadResult.PublicId);
            }
            return Errors.RequestInsertionFailed;
        }
    }
}
