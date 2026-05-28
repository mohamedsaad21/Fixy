using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Serilog;

namespace Fixy.Application.Features.Admin.Queries.GetBookings;

public sealed class GetBookingsQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetBookingsQuery, Result<PaginatedResult<GetBookingsResponse>>>
{
    public async Task<Result<PaginatedResult<GetBookingsResponse>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Admin fetching bookings list. Page: {PageNumber}, PageSize: {PageSize}, Status: {Status}, FromDate: {FromDate}, ToDate: {ToDate}, Search: {Search}, OrderBy: {OrderBy}, SortOrder: {SortOrder}",
            request.PageNumber, request.PageSize, request.Status, request.FromDate, request.ToDate, request.Search, request.OrderBy, request.SortOrder);

        var query = unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.CreatedAt >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.CreatedAt <= request.ToDate);

        query = (request.OrderBy, request.SortOrder) switch
        {
            (BookingOrdering.ScheduledDateTime, SortOrderOptions.ASC) => query.OrderBy(x => x.ScheduledDateTime),
            (BookingOrdering.ScheduledDateTime, SortOrderOptions.DESC) => query.OrderByDescending(x => x.ScheduledDateTime),
            (BookingOrdering.CreatedAt, SortOrderOptions.ASC) => query.OrderBy(x => x.CreatedAt),
            (BookingOrdering.CreatedAt, SortOrderOptions.DESC) => query.OrderByDescending(x => x.CreatedAt),
            _ => query
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.ServiceRequest.Customer.UserName.Contains(request.Search) 
            || x.Technician.UserName.Contains(request.Search));
        }

        var data = await query
            .Select(x => new GetBookingsResponse
            {
                Id = x.Id,
                CustomerId = x.ServiceRequest.CustomerId,
                TechnicianId = x.TechnicianId,
                CustomerName = x.ServiceRequest.Customer.FirstName + " " + x.ServiceRequest.Customer.LastName,
                TechnicianName = x.Technician.FirstName + " " + x.Technician.LastName,
                CustomerUserName = x.Technician.UserName,
                TechnicianUserName = x.Technician.UserName,
                Price = x.AgreedPrice,
                Status = EnumLocalizer.Localize(x.Status, localizer),
                CreatedAt = x.CreatedAt.ToEgyptTime(),
                CompletedAt = x.CompletedAt
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        Log.Information("Bookings list fetched successfully. TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", data.TotalCount, data.CurrentPage, data.PageSize);
        return data;
    }
}
