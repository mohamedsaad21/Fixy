using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Helpers;
using Fixy.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Features.Authentication.Commands.Handlers;

public class AuthenticationCommandsHandler : IRequestHandler<RegisterCustomerCommand, Result>,
                                                                                      IRequestHandler<SignInCommand, Result<AuthResponse>>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    public AuthenticationCommandsHandler(IStringLocalizer<SharedResources> stringLocalizer, 
        IAuthenticationService authenticationService, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _stringLocalizer = stringLocalizer;
        _authenticationService = authenticationService;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByNameAsync(request.UserName) != null)
            return Errors.UserNameAlreadyExists;

        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        var customer = _mapper.Map<Customer>(request);

        var result = await _userManager.CreateAsync(customer, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += $"{error.Description},";
            }
            return Errors.IdentityFailure(errors);
        }

        await _userManager.AddToRoleAsync(customer, Roles.Customer);
        // Generate confirm email code
        var random = new Random();
        var code = random.Next(1, 1000000).ToString("D6");
        customer.Code = code;
        await _userManager.UpdateAsync(customer);
        // send code to customer to verify email
        var message = $"This code to confirm your account: {code}";
        await _emailService.SendEmailAsync(customer.Email, message, "Confirm Account");
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Errors.EmailOrPasswordInCorrect;

        if (!user.EmailConfirmed)
            return Errors.EmailNotConfirmed;

        // Get Jwt Token
        var jwtSecurityToken =  await _authenticationService.CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var roles = await _userManager.GetRolesAsync(user);
        var authResponse = new AuthResponse(user.UserName, user.Email, roles.ToList(),accessToken, jwtSecurityToken.ValidTo);
        return authResponse;
    }
}