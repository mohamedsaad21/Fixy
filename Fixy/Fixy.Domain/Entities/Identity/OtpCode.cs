using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Fixy.Domain.Entities.Identity;

public class OtpCode : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    [EncryptColumn]
    public string Code { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
