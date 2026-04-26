using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetTechnicians;

public sealed class GetTechniciansQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTechniciansQuery, Result<PaginatedResult<GetTechniciansResponse>>>
{
    public async Task<Result<PaginatedResult<GetTechniciansResponse>>> Handle(GetTechniciansQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Technicians.GetTableNoTracking();

        query = request.OrderBy switch
        {
            TechnicianOrdering.AverageRating => query.OrderBy(x => x.AverageRating),
            TechnicianOrdering.TotalCompletedJobs => query.OrderBy(x => x.CompletedBookings),
            TechnicianOrdering.CancellationRate => query.OrderBy(x => x.CancellationRate),
            _ => query
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.FirstName.Contains(request.Search) || x.LastName.Contains(request.Search)
            || x.UserName.Contains(request.Search) || x.Email.Contains(request.Search));
        }


        var technicians = await query.Select(x => new GetTechniciansResponse
        {
            Id = x.Id,
            FullName = x.FirstName + " " + x.LastName,
            UserName = x.UserName,
            Email = x.Email,
            Status = x.Status.ToString(),
            TotalCompletedJobs = x.CompletedBookings,
            CancellationRate = x.CancellationRate,
            AverageRating = x.AverageRating
        }).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return technicians;
    }
}
