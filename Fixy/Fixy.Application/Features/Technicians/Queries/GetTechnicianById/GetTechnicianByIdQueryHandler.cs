using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianById;

public sealed class GetTechnicianByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetTechnicianByIdQuery, Result<GetTechnicianByIdResponse>>
{
    public async Task<Result<GetTechnicianByIdResponse>> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
    {
        var technician = await userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (technician == null)
            return Errors.TechnicianNotFound;

        var technicianResponse = mapper.Map<GetTechnicianByIdResponse>(technician);
        return technicianResponse;
    }
}
