using Fixy.Domain.Constants;
using Fixy.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Infrastructure.Seeder;

public static class UserSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            FullName = "Mohamed Saad",
            UserName = "admin",
            Email = "admin@fixy.com",
            EmailConfirmed = true,
        };
        var password = "Ad@123";

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}
