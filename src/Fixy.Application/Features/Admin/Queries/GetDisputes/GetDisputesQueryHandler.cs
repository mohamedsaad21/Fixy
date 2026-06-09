using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Queries.GetDisputes;

public class GetDisputesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDisputesQuery, Result<PaginatedResult<DisputeDto>>>
{
    public async Task<Result<PaginatedResult<DisputeDto>>> Handle(GetDisputesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Disputes.GetTableNoTracking()
            .Include(x => x.Raiser)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new DisputeDto
            {
                Id = x.Id,
                BookingId = x.ServiceBookingId,
                RaiserName = $"{x.Raiser.FirstName} {x.Raiser.LastName}",
                Reason = x.Reason,
                DesiredResolution = x.DesiredResolution,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = PaginatedResult<DisputeDto>.Success(items, count, request.PageNumber, request.PageSize);
        return paginatedResult;
    }
}
