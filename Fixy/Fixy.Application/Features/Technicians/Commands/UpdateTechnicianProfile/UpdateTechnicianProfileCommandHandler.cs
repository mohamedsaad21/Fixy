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

public sealed class UpdateTechnicianProfileCommandHandler(IUnitOfWork unitOfWork, IFileService fileService) : IRequestHandler<UpdateTechnicianProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateTechnicianProfileCommand request, CancellationToken cancellationToken)
    {
        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.TechnicianId);

        if (technician == null)
            return Errors.TechnicianNotFound;

        technician.NationalId = request.NationalId;

        await fileService.DeleteAsync(technician.NationalIdCardImagePublicId);
        technician.NationalIdCardImageUrl = null;
        technician.NationalIdCardImagePublicId = null;

        var nationalIdResult = await fileService.UploadAsync($"Technicians/{technician.Id}/NationalIds", request.NationalIdCardImage);

        if (!nationalIdResult.IsSuccess)
            return Errors.NationalIdUploadFailed;

        technician.NationalIdCardImageUrl = nationalIdResult.Url;
        technician.NationalIdCardImagePublicId = nationalIdResult.PublicId;

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
