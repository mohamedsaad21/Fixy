using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Queries.GetDisputeById;

public class GetDisputeByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDisputeByIdQuery, Result<DisputeDetailsDto>>
{
    public async Task<Result<DisputeDetailsDto>> Handle(GetDisputeByIdQuery request, CancellationToken cancellationToken)
    {
        var dispute = await unitOfWork.Disputes.GetTableNoTracking()
            .Include(x => x.Raiser)
            .Include(x => x.Resolver)
            .Include(x => x.ServiceBooking).ThenInclude(x => x.Technician)
            .Include(x => x.ServiceBooking).ThenInclude(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.DisputeId, cancellationToken);

        if (dispute == null)
            return Errors.DisputeNotFound;

        var dto = new DisputeDetailsDto
        {
            Id = dispute.Id,
            BookingId = dispute.ServiceBookingId,
            TechnicianId = dispute.ServiceBooking.TechnicianId,
            TechnicianName = $"{dispute.ServiceBooking.Technician.FirstName} {dispute.ServiceBooking.Technician.LastName}",
            CustomerId = dispute.ServiceBooking.ServiceRequest.CustomerId,
            CustomerName = $"{dispute.ServiceBooking.ServiceRequest.Customer.FirstName} {dispute.ServiceBooking.ServiceRequest.Customer.LastName}",
            RaiserId = dispute.RaiserId,
            RaiserName = $"{dispute.Raiser.FirstName} {dispute.Raiser.LastName}",
            Reason = dispute.Reason,
            DesiredResolution = dispute.DesiredResolution,
            Status = dispute.Status,
            ResolutionOutcome = dispute.ResolutionOutcome,
            CreatedAt = dispute.CreatedAt,
            ResolvedAt = dispute.ResolvedAt,
            ResolverName = dispute.Resolver != null ? $"{dispute.Resolver.FirstName} {dispute.Resolver.LastName}" : null
        };

        return dto;
    }
}
