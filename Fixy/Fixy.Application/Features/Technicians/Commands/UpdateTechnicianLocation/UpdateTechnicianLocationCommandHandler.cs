using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;

public sealed class UpdateTechnicianLocationCommandHandler : IRequestHandler<UpdateTechnicianLocationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTechnicianLocationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateTechnicianLocationCommand request, CancellationToken cancellationToken)
    {
        var technicianId = _currentUserService.GetCurrentUserId();
        var location = await _unitOfWork.TechnicianLocations.GetTableNoTracking().Where(x => x.TechnicianId == technicianId).FirstOrDefaultAsync();

        if (location == null)
        {
            location = new TechnicianLocation(technicianId, request.Latitude, request.Longitude);
            await _unitOfWork.TechnicianLocations.AddAsync(location);
        }
        else
        {
            location.Update(request.Latitude, request.Longitude);
        }
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
