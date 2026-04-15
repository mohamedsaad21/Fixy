using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace Fixy.Application.Features.Authentication.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper,
    IFileService fileService, IAuthenticationService authenticationService)
    : IRequestHandler<RegisterCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        var customer = mapper.Map<Customer>(request);
        customer.UserName = new MailAddress(request.Email).User;

        string? profilePicturePublicId = null;
        string? nationalIdCardImagePublicId = null;

        try
        {
            if (request.ProfilePicture != null)
            {
                var ProfileResult = await fileService.UploadAsync($"Customers/{customer.Id}/Profiles", request.ProfilePicture);

                if (!ProfileResult.IsSuccess)
                    throw new Exception("Profile Upload Failed");

                customer.ProfilePictureUrl = ProfileResult.Url;
                customer.ProfilePicturePublicId = ProfileResult.PublicId;
                profilePicturePublicId = ProfileResult.PublicId;
            }

            var createResult = await userManager.CreateAsync(customer, request.Password);
            if (!createResult.Succeeded)
                return Errors.IdentityCreateUserFailed;

            var roleResult = await userManager.AddToRoleAsync(customer, Roles.Customer);
            if (!roleResult.Succeeded)
                return Errors.IdentityAddRoleFailed;

            await unitOfWork.Wallets.AddAsync(new Wallet
            {
                ApplicationUserId = customer.Id,
                Balance = 0
            });
            await unitOfWork.SaveChangesAsync();

            await authenticationService.SendOtpAsync(customer, "confirm your account", "Confirm Account");
            return customer.Id;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(profilePicturePublicId))
                await fileService.DeleteAsync(profilePicturePublicId);

            if (!string.IsNullOrWhiteSpace(nationalIdCardImagePublicId))
                await fileService.DeleteAsync(nationalIdCardImagePublicId);

            await userManager.DeleteAsync(customer);

            return Errors.FileUploadFailed;
        }
    }
}
