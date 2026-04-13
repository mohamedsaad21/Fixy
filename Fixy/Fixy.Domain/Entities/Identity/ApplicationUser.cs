using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        Notifications = new HashSet<Notification>();
    }
    public string FullName { get; set; }
    [EncryptColumn]
    public string? Code { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
<<<<<<< HEAD
=======
    public bool IsTwoFactorEmailEnabled { get; set; }
>>>>>>> feature/MFA
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
}
