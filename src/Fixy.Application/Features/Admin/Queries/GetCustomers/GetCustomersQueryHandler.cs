using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Admin.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetCustomersQuery, Result<PaginatedResult<GetCustomersResponse>>>
{
    public async Task<Result<PaginatedResult<GetCustomersResponse>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Customers.GetTableNoTracking().AsQueryable();

        query = request.OrderBy switch
        {
            _ => query
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.FirstName.Contains(request.Search) || x.LastName.Contains(request.Search)
            || x.UserName.Contains(request.Search) || x.Email.Contains(request.Search));
        }


        var technicians = await query.Select(x => new GetCustomersResponse
        {
            Id = x.Id,
            FullName = x.FirstName + " " + x.LastName,
            UserName = x.UserName,
            Email = x.Email,
            Status = EnumLocalizer.Localize(x.Status, localizer),
            TotalBookings = x.TotalBookings,
            CompletedBookings = x.CompletedBookings,
            CancelledBookings = x.CancelledBookings,
            CancellationRate = x.CancellationRate,
        }).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return technicians;
    }
}
