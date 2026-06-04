using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Queries.GetTechnicians;

public sealed class GetTechniciansQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer, ILogger<GetTechniciansQueryHandler> logger) : IRequestHandler<GetTechniciansQuery, Result<PaginatedResult<GetTechniciansResponse>>>
{
    public async Task<Result<PaginatedResult<GetTechniciansResponse>>> Handle(GetTechniciansQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin fetching technicians list. Page: {PageNumber}, PageSize: {PageSize}, Search: {Search}, OrderBy: {OrderBy}, SortOrder: {SortOrder}",
            request.PageNumber, request.PageSize, request.Search, request.OrderBy, request.SortOrder);

        var query = unitOfWork.Technicians.GetTableNoTracking();

        query = (request.OrderBy, request.SortOrder) switch
        {
            (TechnicianOrdering.AverageRating, SortOrderOptions.ASC) => query.OrderBy(x => x.AverageRating),
            (TechnicianOrdering.AverageRating, SortOrderOptions.DESC) => query.OrderByDescending(x => x.AverageRating),
            (TechnicianOrdering.TotalCompletedJobs, SortOrderOptions.ASC) => query.OrderBy(x => x.CompletedBookings),
            (TechnicianOrdering.TotalCompletedJobs, SortOrderOptions.DESC) => query.OrderByDescending(x => x.CompletedBookings),
            (TechnicianOrdering.CancellationRate, SortOrderOptions.ASC) => query.OrderBy(x => x.CancellationRate),
            (TechnicianOrdering.CancellationRate, SortOrderOptions.DESC) => query.OrderByDescending(x => x.CancellationRate),
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
            Status = EnumLocalizer.Localize(x.Status, localizer),
            TotalCompletedJobs = x.CompletedBookings,
            CancellationRate = x.CancellationRate,
            AverageRating = x.AverageRating
        }).ToPaginatedListAsync(request.PageNumber, request.PageSize);

        logger.LogInformation("Technicians list fetched successfully. TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", technicians.TotalCount, technicians.CurrentPage, technicians.PageSize);

        return technicians;
    }
}
