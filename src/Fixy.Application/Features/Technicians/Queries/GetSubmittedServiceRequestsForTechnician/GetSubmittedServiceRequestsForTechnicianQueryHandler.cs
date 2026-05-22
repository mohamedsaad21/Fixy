using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Technicians.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetSubmittedServiceRequestsForTechnician;

public sealed class GetSubmittedServiceRequestsForTechnicianQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    : IRequestHandler<GetSubmittedServiceRequestsForTechnicianQuery, Result<PaginatedResult<GetSubmittedServiceRequestsForTechnicianResponse>>>
{
    public async Task<Result<PaginatedResult<GetSubmittedServiceRequestsForTechnicianResponse>>> Handle(GetSubmittedServiceRequestsForTechnicianQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var query = unitOfWork.ServiceRequests.GetTableNoTracking().Include(x => x.ServiceCategories)
            .Include(x => x.PriceOffers)
            .Where(x => x.PriceOffers.Any(x => x.TechnicianId == technician.Id));

        var paginaedList = await query.Select(x => x.ToGetSubmittedServiceRequestsForTechnicianResponse(technician.Id)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return paginaedList;
    }
}
