using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs.PriceOffer;
using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetCustomerServiceRequestByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IStringLocalizer<SharedResources> localizer)
    : IRequestHandler<GetCustomerServiceRequestByIdQuery, Result<GetCustomerServiceRequestByIdResponse>>
{
    public async Task<Result<GetCustomerServiceRequestByIdResponse>> Handle(
        GetCustomerServiceRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var serviceRequest = await unitOfWork.ServiceRequests
            .GetTableNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCustomerServiceRequestByIdResponse   // ✅ project in DB, don't load entities
            {
                Id = x.Id,
                CustomerUserName = x.Customer.UserName,
                Description = x.Description,
                ScheduledDateTime = x.ScheduledDateTime,
                Status = EnumLocalizer.Localize(x.Status, localizer),
                ServiceCategories = x.ServiceCategories
                                     .Select(c => c.Localize(c.NameAr, c.NameEn))
                                     .ToList(),
                Address = new AddressDto(
                                     x.Address.Country, x.Address.City, x.Address.Area,
                                     x.Address.Street, x.Address.BuildingNumber,
                                     x.Address.Latitude, x.Address.Longitude),
                Images = x.ServiceRequestImages == null? new List<ImageDto>() :
                                     x.ServiceRequestImages
                                     .Select(i => new ImageDto { Id = i.Id, ImageUrl = i.ImageUrl })
                                     .ToList(),
                PriceOffers = x.Status == ServiceRequestStatus.Assigned

                    // Only the accepted offer — minimal data
                    ? x.PriceOffers
                       .Where(p => p.Status == PriceOfferStatus.Accepted)
                       .Select(p => new PriceOfferDto
                       {
                           Id = p.Id,
                           TechnicianId = p.Technician.Id,
                           TechnicianUserName = p.Technician.UserName,
                           TechnicianFullName = p.Technician.FirstName + " " + p.Technician.LastName,
                           TechnicianCategory = p.Technician.ServiceCategory.Localize(
                                                    p.Technician.ServiceCategory.NameAr,
                                                    p.Technician.ServiceCategory.NameEn),
                           AverageRating = p.Technician.AverageRating,
                           Price = p.Price,
                           DistanceKm = HaversineDistance.CalculateDistance(
                                                    p.Technician.TechnicianLocation.Latitude,
                                                    p.Technician.TechnicianLocation.Longitude,
                                                    x.Address.Latitude,
                                                    x.Address.Longitude),
                           CreatedAt = p.CreatedAt
                       }).ToList()

                    // All offers — sorted in DB by rating and price (distance still in memory)
                    : x.PriceOffers
                       .Select(p => new PriceOfferDto
                       {
                           Id = p.Id,
                           TechnicianId = p.Technician.Id,
                           TechnicianUserName = p.Technician.UserName,
                           TechnicianFullName = p.Technician.FirstName + " " + p.Technician.LastName,
                           TechnicianCategory = p.Technician.ServiceCategory.Localize(
                                                    p.Technician.ServiceCategory.NameAr,
                                                    p.Technician.ServiceCategory.NameEn),
                           AverageRating = p.Technician.AverageRating,
                           Price = p.Price,
                           DistanceKm = HaversineDistance.CalculateDistance(
                                                    p.Technician.TechnicianLocation.Latitude,
                                                    p.Technician.TechnicianLocation.Longitude,
                                                    x.Address.Latitude,
                                                    x.Address.Longitude),
                           CreatedAt = p.CreatedAt
                       })
                       .OrderByDescending(p => p.AverageRating)
                       .ThenBy(p => p.Price)    // ⚠️ DistanceKm sort moved below — see note
                       .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (serviceRequest is null)
            return Errors.ServiceRequestNotFound;

        // ✅ Sort by distance in-memory AFTER projection (Haversine can't translate to SQL)
        if (serviceRequest.Status != EnumLocalizer.Localize(ServiceRequestStatus.Assigned, localizer))
            serviceRequest.PriceOffers = serviceRequest.PriceOffers
                .OrderByDescending(p => p.AverageRating)
                .ThenBy(p => p.DistanceKm)
                .ThenBy(p => p.Price)
                .ToList();

        return serviceRequest;
    }
}
