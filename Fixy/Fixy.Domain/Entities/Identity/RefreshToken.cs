using Microsoft.EntityFrameworkCore;

namespace Fixy.Domain.Entities.Identity;

[Owned]
public class RefreshToken
{
    public string Token {  get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOn;
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? RevokedOn { get; set; }
    public bool IsActive => RevokedOn == null && !IsExpired;
}
