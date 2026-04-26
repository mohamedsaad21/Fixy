using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;

public sealed class GetTechnicianProfileForCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetTechnicianProfileForCustomersQuery, Result<GetTechnicianProfileForCustomersResponse>>
{
    public async Task<Result<GetTechnicianProfileForCustomersResponse>> Handle(GetTechnicianProfileForCustomersQuery request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .Include(x => x.ServiceCategory).Include(x => x.TechnicianLocation)
            .FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var technicianResponse = mapper.Map<GetTechnicianProfileForCustomersResponse>(technician);
        return technicianResponse;
    }
}
