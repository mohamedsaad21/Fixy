using Fixy.Application.Abstracts;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fixy.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWTSettings _jWTSettings;

    public AuthenticationService(UserManager<ApplicationUser> userManager, JWTSettings jWTSettings)
    {
        _userManager = userManager;
        _jWTSettings = jWTSettings;
    }

    
    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
            roleClaims.Add(new Claim("role", role));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id),
        }.Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer:_jWTSettings.Issuer,
            audience:_jWTSettings.Audience,
            claims:claims,
            expires:DateTime.UtcNow.AddMinutes(_jWTSettings.DurationInMinutes),
            signingCredentials:signingCredentials
            );

        return jwtSecurityToken;
    }

}
