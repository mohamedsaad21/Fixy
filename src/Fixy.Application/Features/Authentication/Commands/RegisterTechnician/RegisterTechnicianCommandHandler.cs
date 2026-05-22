using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace Fixy.Application.Features.Authentication.Commands.RegisterTechnician;

public sealed class RegisterTechnicianCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, 
    IMapper mapper, IStorageService fileService, IAuthenticationService authenticationService)
    : IRequestHandler<RegisterTechnicianCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterTechnicianCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        if (await unitOfWork.Technicians.NationalIdExistsAsync(request.NationalId))
            return Errors.NationalIdAlreadyExists;

        var technician = mapper.Map<Technician>(request);
        technician.UserName = new MailAddress(request.Email).User;

        try
        {
            if (request.ProfilePicture != null)
            {
                var profilePictureUrl = await fileService.UploadAsync(request.ProfilePicture);
                technician.ProfilePictureUrl = profilePictureUrl;
            }

            var nationalIdPictureUrl = await fileService.UploadAsync(request.NationalIdCardImage);

            technician.NationalIdCardImageUrl = nationalIdPictureUrl;

            var createResult = await userManager.CreateAsync(technician, request.Password);
            if (!createResult.Succeeded)
                return Errors.IdentityCreateUserFailed;

            var roleResult = await userManager.AddToRoleAsync(technician, Roles.Technician);
            if (!roleResult.Succeeded)
                return Errors.IdentityAddRoleFailed;

            BackgroundJob.Enqueue<IAuthenticationService>(x => x.SendOtpAsync(technician, "confirm your account", "Confirm Account"));
            return technician.Id;
        }
        catch (Exception)
        {
            await userManager.DeleteAsync(technician);

            return Errors.FileUploadFailed;
        }
    }
}
