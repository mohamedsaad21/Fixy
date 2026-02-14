using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Common.Models;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CreateServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, 
        IFileService fileService, IMapper mapper, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileService = fileService;
        _mapper = mapper;
        _notificationService = notificationService;
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
            // Notify nearby technicians in same category
            foreach(var category in serviceRequest.ServiceCategories)
            {
                var technicians = await _unitOfWork.Technicians.GetTableNoTracking().Where(x => x.ServiceCategoryId == category.Id && x.IsActive).Include(x => x.TechnicianLocation).ToListAsync();
                var nearbyTechnicians = technicians.Select(x => new
                {
                    Technician = x,
                    Distance = GeoDistance.CalculateKm(x.TechnicianLocation.Latitude, x.TechnicianLocation.Longitude, request.Address.Latitude, request.Address.Longitude)
                })
                    .Where(x => x.Distance <= 25)
                    .OrderBy(x => x.Distance)
                    .ThenByDescending(x => x.Technician.AverageRating).ThenByDescending(x => x.Technician.TotalCompletedJobs);
                foreach (var technician in nearbyTechnicians.Take(10))
                {
                    await _notificationService.NotifyNewServiceRequestAsync(technician.Technician.Id, new
                    {
                        serviceRequestId = serviceRequest.Id,
                        title = serviceRequest.Description,
                        distance = technician.Distance,
                        categoryName = category.Name,
                        createdAt = serviceRequest.CreatedAt
                    });
                }
            }
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
