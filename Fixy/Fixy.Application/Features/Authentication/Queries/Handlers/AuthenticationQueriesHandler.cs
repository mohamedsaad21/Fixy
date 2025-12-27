using AutoMapper;
using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.Queries.Models;
using Fixy.Application.Features.Authentication.Queries.Results;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Queries.Handlers;

public class AuthenticationQueriesHandler : IRequestHandler<GetCustomersQuery, Result<List<GetCustomersResponse>>>,
                                                                                IRequestHandler<ConfirmResetPasswordQuery, Result>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    public AuthenticationQueriesHandler(IStringLocalizer<SharedResources> stringLocalizer,
        IAuthenticationService authenticationService, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _stringLocalizer = stringLocalizer;
        _authenticationService = authenticationService;
        _mapper = mapper;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result<List<GetCustomersResponse>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _userManager.Users.Where(u => u.GetType() == typeof(Customer)).ToListAsync();
        var result = _mapper.Map<List<GetCustomersResponse>>(customers);
        return result;
    }

    public async Task<Result> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (user.Code != request.Code)
            return Errors.InvalidCode;

        return Result.Success();
    }
}