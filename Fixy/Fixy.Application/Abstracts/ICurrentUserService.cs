using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Abstracts;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    Task<ApplicationUser> GetCurrentUserAsync();
    Task<List<string>> GetCurrentUserRolesAsync();
}
