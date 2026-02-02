using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
    }
    public string FullName { get; set; }
    [EncryptColumn]
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
