using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public class ApproveTechnicianCommandHandler : IRequestHandler<ApproveTechnicianCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveTechnicianCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await _unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.IsActive)
            return Errors.TechnicianAlreadyApproved;

        technician.IsActive = true;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
