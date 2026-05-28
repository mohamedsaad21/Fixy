using System.Net.Mail;
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
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.RegisterTechnician;

public sealed class RegisterTechnicianCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, 
    IMapper mapper, IStorageService fileService, IAuthenticationService authenticationService)
    : IRequestHandler<RegisterTechnicianCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterTechnicianCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Technician registration initiated. Email: {Email}", request.Email);

        if (await userManager.FindByEmailAsync(request.Email) != null)
        {
            Log.Warning("Technician registration failed — email already exists. Email: {Email}", request.Email);
            return Errors.EmailAlreadyExists;
        }

        if (await unitOfWork.Technicians.NationalIdExistsAsync(request.NationalId))
        {
            Log.Warning("Technician registration failed — National ID already registered. Email: {Email}", request.Email);
            return Errors.NationalIdAlreadyExists;
        }

        var technician = mapper.Map<Technician>(request);
        technician.UserName = new MailAddress(request.Email).User;

        try
        {
            if (request.ProfilePicture != null)
            {
                Log.Information("Uploading technician profile picture. Email: {Email}", request.Email);
                var profilePictureUrl = await fileService.UploadAsync(request.ProfilePicture);
                technician.ProfilePictureUrl = profilePictureUrl;
                Log.Information("Technician profile picture uploaded successfully. Email: {Email}", request.Email);
            }

            Log.Information("Uploading National ID card image. Email: {Email}", request.Email);
            var nationalIdPictureUrl = await fileService.UploadAsync(request.NationalIdCardImage);

            technician.NationalIdCardImageUrl = nationalIdPictureUrl;
            Log.Information("National ID card image uploaded successfully. Email: {Email}", request.Email);

            var createResult = await userManager.CreateAsync(technician, request.Password);
            if (!createResult.Succeeded)
            {
                Log.Warning("Technician registration failed — Identity user creation error. Email: {Email}, Errors: {Errors}", request.Email, string.Join(", ", createResult.Errors.Select(e => e.Code)));
                return Errors.IdentityCreateUserFailed;
            }

            var roleResult = await userManager.AddToRoleAsync(technician, Roles.Technician);
            if (!roleResult.Succeeded)
            {
                Log.Warning("Technician registration failed — role assignment error. UserId: {UserId}, Errors: {Errors}", technician.Id, string.Join(", ", roleResult.Errors.Select(e => e.Code)));
                return Errors.IdentityAddRoleFailed;
            }

            BackgroundJob.Enqueue<IAuthenticationService>(x => x.SendOtpAsync(technician, "confirm your account", "Confirm Account"));
            Log.Information("Technician registered successfully. UserId: {UserId}, Email: {Email}", technician.Id, request.Email);
            return technician.Id;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Technician registration failed — unhandled exception. Email: {Email}", request.Email);

            if (!string.IsNullOrWhiteSpace(technician.ProfilePictureUrl))
            {
                Log.Information("Rolling back profile picture upload. Email: {Email}", request.Email);
                await fileService.DeleteAsync(technician.ProfilePictureUrl);
            }

            if (!string.IsNullOrWhiteSpace(technician.NationalIdCardImageUrl))
            {
                Log.Information("Rolling back National ID card image upload. Email: {Email}", request.Email);
                await fileService.DeleteAsync(technician.NationalIdCardImageUrl);
            }

            await userManager.DeleteAsync(technician);
            Log.Information("Rolled back user creation after failed technician registration. Email: {Email}", request.Email);

            return Errors.FileUploadFailed;
        }
    }
}