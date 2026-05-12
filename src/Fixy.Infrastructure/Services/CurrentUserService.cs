using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<Guid> GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "uid").Value;
        if(userId == null)
        {
            throw new UnauthorizedAccessException();
        }
        return Guid.Parse(userId);
    }

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var userId = await GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user == null)
        {
            throw new UnauthorizedAccessException();
        }
        return user;
    }

    public async Task<string> GetCurrentUserRoleAsync()
    {
        var user = await GetCurrentUserAsync();
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }
}
