using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    [EncryptColumn]
    public string? Code { get; set; }
}
