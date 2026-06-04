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

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;

public sealed class GetBookingsForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer, ILogger<GetBookingsForCustomerQueryHandler>logger) : IRequestHandler<GetBookingsForCustomerQuery, Result<PaginatedResult<GetBookingsForCustomerResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsForCustomerResponse>>> Handle(GetBookingsForCustomerQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer fetching their bookings list. Page: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser is not Customer customer)
        {
            logger.LogWarning("Bookings fetch failed — current user is not a customer. UserId: {UserId}", currentUser?.Id);
            return Errors.Unauthorized;
        }

        var bookings = unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest)
            .Where(x => x.ServiceRequest.CustomerId == customer.Id
            && x.Status != ServiceBookingStatus.CancelledByTechnician).AsQueryable();

        var result = await bookings.Select(x => x.ToGetBookingsForCustomerResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        logger.LogInformation("Customer bookings list fetched successfully. CustomerId: {CustomerId}, TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", customer.Id, result.TotalCount, result.CurrentPage, result.PageSize);
        return result;
    }
}
