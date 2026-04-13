namespace Fixy.Domain.Entities.Identity;

public class OtpCode : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
