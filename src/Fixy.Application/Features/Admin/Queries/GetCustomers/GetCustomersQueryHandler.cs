using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer, ILogger<GetCustomersQueryHandler> logger) : IRequestHandler<GetCustomersQuery, Result<PaginatedResult<GetCustomersResponse>>>
{
    public async Task<Result<PaginatedResult<GetCustomersResponse>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin fetching customers list. Page: {PageNumber}, PageSize: {PageSize}, Search: {Search}, OrderBy: {OrderBy}",
            request.PageNumber, request.PageSize, request.Search, request.OrderBy);

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


        var customers = await query.Select(x => new GetCustomersResponse
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

        logger.LogInformation("Customers list fetched successfully. TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", customers.TotalCount, customers.CurrentPage, customers.PageSize);
        return customers;
    }
}
