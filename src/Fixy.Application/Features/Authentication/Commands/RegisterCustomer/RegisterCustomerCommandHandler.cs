using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace Fixy.Application.Features.Authentication.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper,
    IStorageService fileService, IAuthenticationService authenticationService)
    : IRequestHandler<RegisterCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        var customer = mapper.Map<Customer>(request);
        customer.UserName = new MailAddress(request.Email).User;

        string? profilePicturePublicId = null;
        try
        {
            if (request.ProfilePicture != null)
            {
                var profilePictureUrl = await fileService.UploadAsync(request.ProfilePicture);
                customer.ProfilePictureUrl = profilePictureUrl;
            }

            var createResult = await userManager.CreateAsync(customer, request.Password);
            if (!createResult.Succeeded)
                return Errors.IdentityCreateUserFailed;

            var roleResult = await userManager.AddToRoleAsync(customer, Roles.Customer);
            if (!roleResult.Succeeded)
                return Errors.IdentityAddRoleFailed;

            //await _userManager.UpdateAsync(technician);
            await authenticationService.SendOtpAsync(customer, "confirm your account", "Confirm Account");
            return customer.Id;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(profilePicturePublicId))
                await fileService.DeleteAsync(profilePicturePublicId);

            await userManager.DeleteAsync(customer);

            return Errors.FileUploadFailed;
        }
    }
}
