using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;

public sealed class GetBookingsForTechnicianQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer, ILogger<GetBookingsForTechnicianQueryHandler>logger) : IRequestHandler<GetBookingsForTechnicianQuery, Result<PaginatedResult<GetBookingsForTechnicianResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsForTechnicianResponse>>> Handle(GetBookingsForTechnicianQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician fetching their bookings list. Page: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Technician technician)
        {
            logger.LogWarning("Bookings fetch failed — current user is not a technician. UserId: {UserId}", currentUser?.Id);
            return Errors.Unauthorized;
        }

        var bookings = unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest)
            .Where(x => x.TechnicianId == technician.Id && x.Status != ServiceBookingStatus.CancelledByCustomer).AsQueryable();

        var result = await bookings.Select(x => x.ToGetBookingsForTechnicianResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        logger.LogInformation("Technician bookings list fetched successfully. TechnicianId: {TechnicianId}, TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", technician.Id, result.TotalCount, result.CurrentPage, result.PageSize);
        return result;
    }
}