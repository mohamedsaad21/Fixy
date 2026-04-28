using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianById;

public sealed class GetTechnicianByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetTechnicianByIdQuery, Result<GetTechnicianByIdResponse>>
{
    public async Task<Result<GetTechnicianByIdResponse>> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .Include(x => x.ServiceCategory).Include(x => x.TechnicianLocation)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var technicianResponse = mapper.Map<GetTechnicianByIdResponse>(technician);
        technicianResponse.Status = EnumLocalizer.Localize(technician.Status, localizer);
        return technicianResponse;
    }
}
