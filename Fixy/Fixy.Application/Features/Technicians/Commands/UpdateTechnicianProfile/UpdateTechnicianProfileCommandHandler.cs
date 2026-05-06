using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianProfile;

public sealed class UpdateTechnicianProfileCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService) : IRequestHandler<UpdateTechnicianProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateTechnicianProfileCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        technician.NationalId = request.NationalId;

        //await fileService.DeleteAsync(technician.NationalIdCardImagePublicId);
        technician.NationalIdCardImageUrl = null;

        var nationalIdPictureUrl = await fileService.UploadAsync(request.NationalIdCardImage);

        technician.NationalIdCardImageUrl = nationalIdPictureUrl;

        if (technician.Status == TechnicianStatus.Rejected)
        {
            technician.Status = TechnicianStatus.PendingApproval;
            technician.RejectionReason = null;
            technician.RejectedAt = null;
        }

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
