using Microsoft.AspNetCore.Identity;

namespace Fixy.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        Notifications = new HashSet<Notification>();
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
    public string? FcmToken { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
}
