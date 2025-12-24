using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Infrastructure.Seeder;

public static class UserSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        var user = new Admin
        {
            FullName = "Mohamed Saad",
            UserName = "admin",
            Email = "admin@fixy.com"
        };
        var password = "Ad@123";

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}
