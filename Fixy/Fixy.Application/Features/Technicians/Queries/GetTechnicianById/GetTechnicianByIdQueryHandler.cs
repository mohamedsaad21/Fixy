using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianById;

public sealed class GetTechnicianByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetTechnicianByIdQuery, Result<GetTechnicianByIdResponse>>
{
    public async Task<Result<GetTechnicianByIdResponse>> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .Include(x => x.ServiceCategory).Include(x => x.TechnicianLocation)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var technicianResponse = mapper.Map<GetTechnicianByIdResponse>(technician);
        return technicianResponse;
    }
}
