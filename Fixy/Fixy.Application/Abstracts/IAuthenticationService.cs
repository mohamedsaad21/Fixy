using Fixy.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Abstracts;

public interface IAuthenticationService
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    Task<RefreshToken> GenerateRefreshToken();
    Task SendCodeAsync(ApplicationUser user, string actionText, string reason);
}
