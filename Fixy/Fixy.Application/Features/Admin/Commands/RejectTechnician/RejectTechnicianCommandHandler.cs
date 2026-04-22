using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public sealed class RejectTechnicianCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RejectTechnicianCommand, Result>
{
    public async Task<Result> Handle(RejectTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        // 

        return Result.Success();
    }
}
