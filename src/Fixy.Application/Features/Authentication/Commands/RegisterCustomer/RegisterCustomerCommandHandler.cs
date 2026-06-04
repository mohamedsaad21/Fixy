using System.Net.Mail;
using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper,
    IStorageService fileService, IAuthenticationService authenticationService, ILogger<RegisterCustomerCommandHandler> logger)
    : IRequestHandler<RegisterCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer registration initiated. Email: {Email}", request.Email);

        if (await userManager.FindByEmailAsync(request.Email) != null)
        {
            logger.LogWarning("Customer registration failed — email already exists. Email: {Email}", request.Email);
            return Errors.EmailAlreadyExists;
        }

        var customer = mapper.Map<Customer>(request);
        customer.UserName = new MailAddress(request.Email).User;

        string? profilePicturePublicId = null;
        try
        {
            if (request.ProfilePicture != null)
            {
                logger.LogInformation("Uploading profile picture. Email: {Email}", request.Email);
                var profilePictureUrl = await fileService.UploadAsync(request.ProfilePicture);
                customer.ProfilePictureUrl = profilePictureUrl;
                logger.LogInformation("Profile picture uploaded successfully. Email: {Email}", request.Email);
            }

            var createResult = await userManager.CreateAsync(customer, request.Password);
            if (!createResult.Succeeded)
            {
                logger.LogWarning("Customer registration failed — Identity user creation error. Email: {Email}, Errors: {Errors}", request.Email, string.Join(", ", createResult.Errors.Select(e => e.Code)));
                return Errors.IdentityCreateUserFailed;
            }

            var roleResult = await userManager.AddToRoleAsync(customer, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                logger.LogWarning("Customer registration failed — role assignment error. UserId: {UserId}, Errors: {Errors}", customer.Id, string.Join(", ", roleResult.Errors.Select(e => e.Code)));
                return Errors.IdentityAddRoleFailed;
            }

            BackgroundJob.Enqueue<IAuthenticationService>(x => x.SendOtpAsync(customer, "confirm your account", "Confirm Account"));
            logger.LogInformation("Customer registered successfully. UserId: {UserId}, Email: {Email}", customer.Id, request.Email);
            return customer.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Customer registration failed — unhandled exception during registration. Email: {Email}", request.Email);
            if (!string.IsNullOrWhiteSpace(profilePicturePublicId))
            {
                logger.LogInformation("Rolling back profile picture upload. Email: {Email}", request.Email);
                await fileService.DeleteAsync(profilePicturePublicId);
            }

            await userManager.DeleteAsync(customer);
            logger.LogInformation("Rolled back user creation after failed registration. Email: {Email}", request.Email);
            return Errors.FileUploadFailed;
        }
    }
}