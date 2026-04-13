using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace Fixy.Application.Features.Authentication.Commands.RegisterTechnician;

public sealed class RegisterTechnicianCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, 
    IMapper mapper, IFileService fileService, IAuthenticationService authenticationService)
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

        string? profilePicturePublicId = null;
        string? nationalIdCardImagePublicId = null;

        try
        {
            if (request.ProfilePicture != null)
            {
                var ProfileResult = await fileService.UploadAsync($"Technicians/{technician.Id}/Profiles", request.ProfilePicture);

                if (!ProfileResult.IsSuccess)
                    throw new Exception("Profile Upload Failed");

                technician.ProfilePictureUrl = ProfileResult.Url;
                technician.ProfilePicturePublicId = ProfileResult.PublicId;
                profilePicturePublicId = ProfileResult.PublicId;
            }

            var nationalIdResult = await fileService.UploadAsync($"Technicians/{technician.Id}/NationalIds", request.NationalIdCardImage);

            if (!nationalIdResult.IsSuccess)
                throw new Exception("NationalId Upload Failed");

            technician.NationalIdCardImageUrl = nationalIdResult.Url;
            technician.NationalIdCardImagePublicId = nationalIdResult.PublicId;
            nationalIdCardImagePublicId = nationalIdResult.PublicId;

            var createResult = await userManager.CreateAsync(technician, request.Password);
            if (!createResult.Succeeded)
                return Errors.IdentityCreateUserFailed;

            var roleResult = await userManager.AddToRoleAsync(technician, Roles.Technician);
            if (!roleResult.Succeeded)
                return Errors.IdentityAddRoleFailed;

            //await _userManager.UpdateAsync(technician);
<<<<<<< HEAD
            await authenticationService.SendCodeAsync(technician, "confirm your account", "Confirm Account");
=======
            await authenticationService.SendOtpAsync(technician, "confirm your account", "Confirm Account");
>>>>>>> feature/MFA
            return technician.Id;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(profilePicturePublicId))
                await fileService.DeleteAsync(profilePicturePublicId);

            if (!string.IsNullOrWhiteSpace(nationalIdCardImagePublicId))
                await fileService.DeleteAsync(nationalIdCardImagePublicId);

            await userManager.DeleteAsync(technician);

            return Errors.FileUploadFailed;
        }
    }
}
