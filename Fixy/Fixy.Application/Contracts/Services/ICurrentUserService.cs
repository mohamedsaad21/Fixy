using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Contracts.Services;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    Task<ApplicationUser> GetCurrentUserAsync();
    Task<string> GetCurrentUserRoleAsync();
}
