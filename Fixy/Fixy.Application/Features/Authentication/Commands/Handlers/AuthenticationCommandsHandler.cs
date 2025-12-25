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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Features.Authentication.Commands.Handlers;

public class AuthenticationCommandsHandler : IRequestHandler<RegisterCustomerCommand, Result>,
                                                                                      IRequestHandler<SignInCommand, Result<AuthResponse>>,
                                                                                      IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>,
                                                                                      IRequestHandler<RevokeTokenCommand, Result>,
                                                                                      IRequestHandler<ConfirmEmailCommand, Result>,
                                                                                      IRequestHandler<SendConfirmEmailCommand, Result>,
                                                                                      IRequestHandler<SendResetPasswordCommand, Result>,
                                                                                      IRequestHandler<ResetPasswordCommand, Result>
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
        var authResponse = new AuthResponse();

        if(user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken =  user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            authResponse.RefreshToken = activeRefreshToken.Token;
            authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = await _authenticationService.GenerateRefreshToken();
            authResponse.RefreshToken = refreshToken.Token;
            authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        var jwtSecurityToken =  await _authenticationService.CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var roles = await _userManager.GetRolesAsync(user);
        authResponse.UserName = user.UserName;
        authResponse.Email = user.Email;
        authResponse.Roles = roles.ToList();
        authResponse.Token = accessToken;

        return authResponse;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.Token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == request.Token);

        if (!refreshToken.IsActive)
            return Errors.InactiveToken;

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = await _authenticationService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        var jwtSecurityToken = await _authenticationService.CreateJwtToken(user);

        return new AuthResponse
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiresOn
        };
    }

    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.Token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == request.Token);

        if (!refreshToken.IsActive)
            return Errors.InactiveToken;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (user.Code != request.Code)
            return Errors.InvalidCode;

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (user.EmailConfirmed)
            return Errors.EmailAlreadyConfirmed;

        // Generate confirm email code
        var random = new Random();
        var code = random.Next(1, 1000000).ToString("D6");
        user.Code = code;
        await _userManager.UpdateAsync(user);
        // send code to customer to verify email
        var message = $"This code to confirm your account: {code}";
        await _emailService.SendEmailAsync(user.Email, message, "Confirm Account");
        return Result.Success();
    }

    public async Task<Result> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        // Generate reset password code
        var random = new Random();
        var code = random.Next(1, 1000000).ToString("D6");
        user.Code = code;
        await _userManager.UpdateAsync(user);
        // send code to customer to reset password
        var message = $"This code to reset your password: {code}";
        await _emailService.SendEmailAsync(user.Email, message, "Reset Password");
        return Result.Success();
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, request.Password);

        return Result.Success();
    }
}