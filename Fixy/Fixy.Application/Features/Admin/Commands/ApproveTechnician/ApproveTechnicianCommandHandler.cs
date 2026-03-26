using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ApproveTechnicianCommand, Result>
{
    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.IsActive)
            return Errors.TechnicianAlreadyApproved;

        technician.IsActive = true;

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
