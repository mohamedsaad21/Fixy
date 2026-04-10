using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Bookings.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;

public sealed class GetBookingsForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetBookingsForCustomerQuery, Result<PaginatedResult<GetBookingsForCustomerResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsForCustomerResponse>>> Handle(GetBookingsForCustomerQuery request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        var bookings = unitOfWork.Bookings.GetTableNoTracking().AsQueryable();

        var FilterQuery = await bookings.Select(x => x.ToGetBookingsForCustomerResponse()).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return FilterQuery;
    }
}
