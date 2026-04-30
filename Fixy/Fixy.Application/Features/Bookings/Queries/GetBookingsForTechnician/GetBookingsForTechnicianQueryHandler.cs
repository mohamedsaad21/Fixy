using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;

public sealed class GetBookingsForTechnicianQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetBookingsForTechnicianQuery, Result<PaginatedResult<GetBookingsForTechnicianResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsForTechnicianResponse>>> Handle(GetBookingsForTechnicianQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Technician technician)
            return Errors.Unauthorized;

        var bookings = unitOfWork.Bookings.GetTableNoTracking()
            .Where(x => x.TechnicianId == technician.Id).AsQueryable();

        var FilterQuery = await bookings.Select(x => x.ToGetBookingsForTechnicianResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return FilterQuery;
    }
}
