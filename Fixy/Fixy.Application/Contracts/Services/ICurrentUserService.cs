using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Contracts.Services;

public interface ICurrentUserService
{
    Task<Guid> GetCurrentUserId();
    Task<ApplicationUser> GetCurrentUserAsync();
    Task<string> GetCurrentUserRoleAsync();
}
