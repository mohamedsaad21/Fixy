using Fixy.Domain.Entities.Chat;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        Notifications = new HashSet<Notification>();
        CustomerConversations = new HashSet<Conversation>();
        TechnicianConversations = new HashSet<Conversation>();
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }
    public bool IsTwoFactorEmailEnabled { get; set; }
    public string? FcmToken { get; set; }
    public int? TotalBookings { get; set; }
    public int? CompletedBookings { get; set; }
    public int? CancelledBookings { get; set; }
    public double? CancellationRate { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? BlockedAt { get; set; }
    public Guid? BlockedBy { get; set; }
    public ApplicationUser BlockedByUser { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
    public virtual ICollection<Conversation> CustomerConversations { get; set; }
    public virtual ICollection<Conversation> TechnicianConversations { get; set; }
}
