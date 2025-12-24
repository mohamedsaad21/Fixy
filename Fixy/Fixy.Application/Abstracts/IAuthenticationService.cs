using Fixy.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Abstracts;

public interface IAuthenticationService
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
}
