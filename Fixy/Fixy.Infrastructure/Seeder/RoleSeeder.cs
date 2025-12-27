using Fixy.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Infrastructure.Seeder;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { Roles.Admin, Roles.Technician, Roles.Customer };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> {Name = role });
        }
    }
}
