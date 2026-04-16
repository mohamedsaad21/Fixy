using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.Queries.Models;
using Fixy.Application.Features.Authentication.Queries.Results;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Authentication.Queries.Handlers;

public class AuthenticationQueriesHandler : IRequestHandler<GetCustomersQuery, Result<List<GetCustomersResponse>>>                                                                               
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    public AuthenticationQueriesHandler(IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Result<List<GetCustomersResponse>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _userManager.Users.Where(u => u.GetType() == typeof(Customer)).ToListAsync();
        var result = _mapper.Map<List<GetCustomersResponse>>(customers);
        return result;
    }
}