using CloudinaryDotNet.Core;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianLocation;

public class UpdateTechnicianLocationCommandHandler : IRequestHandler<UpdateTechnicianLocationCommand, Result>
{
    private readonly ITechnicianLocationRepository _technicianLocationRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTechnicianLocationCommandHandler(ITechnicianLocationRepository technicianLocationRepository, ICurrentUserService currentUserService)
    {
        _technicianLocationRepository = technicianLocationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateTechnicianLocationCommand request, CancellationToken cancellationToken)
    {
        var technicianId = _currentUserService.GetCurrentUserId();
        var location = await _technicianLocationRepository.GetTableNoTracking().Where(x => x.TechnicianId == technicianId).FirstOrDefaultAsync();

        if (location == null)
        {
            location = new TechnicianLocation(technicianId, request.Latitude, request.Longitude);
            await _technicianLocationRepository.AddAsync(location);
        }
        else
        {
            location.Update(request.Latitude, request.Longitude);
            await _technicianLocationRepository.UpdateAsync(location);
        }
        return Result.Success();
    }
}
