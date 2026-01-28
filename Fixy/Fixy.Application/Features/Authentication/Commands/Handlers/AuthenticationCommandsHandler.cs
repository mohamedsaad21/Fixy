using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Application.Resources;
using Fixy.Domain.Constants;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;

namespace Fixy.Application.Features.Authentication.Commands.Handlers;

public class AuthenticationCommandsHandler : IRequestHandler<RegisterCustomerCommand, Result>,
                                                                                      IRequestHandler<SignInCommand, Result<AuthResponse>>,
                                                                                      IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>,
                                                                                      IRequestHandler<RevokeTokenCommand, Result>,
                                                                                      IRequestHandler<ConfirmEmailCommand, Result>,
                                                                                      IRequestHandler<SendConfirmEmailCommand, Result>,
                                                                                      IRequestHandler<SendResetPasswordCommand, Result>,
                                                                                      IRequestHandler<ResetPasswordCommand, Result>,
                                                                                      IRequestHandler<RegisterTechnicianCommand, Result<Guid>>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ITechnicianRepository _technicianRepository;
    private readonly IFileService _fileService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthenticationCommandsHandler(IStringLocalizer<SharedResources> stringLocalizer, 
        IAuthenticationService authenticationService, IMapper mapper, UserManager<ApplicationUser> userManager,
        IEmailService emailService, ITechnicianRepository technicianRepository, IFileService fileService, IHttpContextAccessor httpContextAccessor)
    {
        _stringLocalizer = stringLocalizer;
        _authenticationService = authenticationService;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
        _technicianRepository = technicianRepository;
        _fileService = fileService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        var customer = _mapper.Map<Customer>(request);
        customer.UserName = new MailAddress(request.Email).User;

        var result = await _userManager.CreateAsync(customer, request.Password);

        if (!result.Succeeded)
            return Errors.IdentityCreateUserFailed;

        await _userManager.AddToRoleAsync(customer, Roles.Customer);
        await _authenticationService.SendCodeAsync(customer, "confirm your account", "Confirm Account");
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

        SetTokenAndRefreshTokenInCookie(accessToken, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

        return authResponse;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return Errors.InactiveToken;

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = await _authenticationService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        var jwtSecurityToken = await _authenticationService.CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        SetTokenAndRefreshTokenInCookie(accessToken, newRefreshToken.Token, newRefreshToken.ExpiresOn);
        var roles = await _userManager.GetRolesAsync(user);
        return new AuthResponse
        {
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles.ToList(),
            Token = accessToken,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiresOn
        };
    }

    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

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
        user.Code = null;
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

        await _authenticationService.SendCodeAsync(user, "confirm your account", "Confirm Account");
        return Result.Success();
    }

    public async Task<Result> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        await _authenticationService.SendCodeAsync(user, "reset your password", "Reset Password");
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

    public async Task<Result<Guid>> Handle(RegisterTechnicianCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Errors.EmailAlreadyExists;

        if (await _technicianRepository.NationalIdExistsAsync(request.NationalId))
            return Errors.NationalIdAlreadyExists;

        var technician = _mapper.Map<Technician>(request);
        technician.UserName = new MailAddress(request.Email).User;

        var createResult = await _userManager.CreateAsync(technician, request.Password);
        if (!createResult.Succeeded)
            return Errors.IdentityCreateUserFailed;

        var roleResult = await _userManager.AddToRoleAsync(technician, Roles.Technician);
        if (!roleResult.Succeeded)
            return Errors.IdentityAddRoleFailed;

        string? profilePicturePublicId = null;
        string? nationalIdCardImagePublicId = null;

        try
        {
            var ProfileResult = await _fileService.UploadAsync($"Technicians/{technician.Id}/Profiles", request.ProfilePicture);

            if (!ProfileResult.IsSuccess)
                throw new Exception("Profile Upload Failed");

            technician.ProfilePictureUrl = ProfileResult.Url;
            technician.ProfilePicturePublicId = ProfileResult.PublicId;
            profilePicturePublicId = ProfileResult.PublicId;

            var nationalIdResult = await _fileService.UploadAsync($"Technicians/{technician.Id}/NationalIds", request.NationalIdCardImage);

            if (!nationalIdResult.IsSuccess)
                throw new Exception("NationalId Upload Failed");

            technician.NationalIdCardImageUrl = nationalIdResult.Url;
            technician.NationalIdCardImagePublicId = nationalIdResult.PublicId;
            nationalIdCardImagePublicId = nationalIdResult.PublicId;

            await _userManager.UpdateAsync(technician);
            await _authenticationService.SendCodeAsync(technician, "confirm your account", "Confirm Account");
            return technician.Id;
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(profilePicturePublicId))
                await _fileService.DeleteAsync(profilePicturePublicId);

            if (!string.IsNullOrWhiteSpace(nationalIdCardImagePublicId))
                await _fileService.DeleteAsync(nationalIdCardImagePublicId);

            await _userManager.DeleteAsync(technician);

            return Errors.FileUploadFailed;
        }
    }

    private void SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTime expires)
    {
        var response = _httpContextAccessor.HttpContext?.Response;

        if (response == null)
            throw new InvalidOperationException("HTTP context is not available");

        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };
        response.Cookies.Append("token", token);
        response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);
    }
}