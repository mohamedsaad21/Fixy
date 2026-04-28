using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;

public sealed class GetBookingsForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetBookingsForCustomerQuery, Result<PaginatedResult<GetBookingsForCustomerResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsForCustomerResponse>>> Handle(GetBookingsForCustomerQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Customer customer)
            return Errors.Unauthorized;

        var bookings = unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest)
            .Where(x => x.ServiceRequest.CustomerId == customer.Id).AsQueryable();

        var FilterQuery = await bookings.Select(x => x.ToGetBookingsForCustomerResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return FilterQuery;
    }
}
