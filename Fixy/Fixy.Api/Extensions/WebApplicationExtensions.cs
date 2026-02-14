using Fixy.Api.Hubs;
using Fixy.Api.Middleware;
using Fixy.Domain.Entities.Identity;
using Fixy.Infrastructure.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Fixy.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseApiPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fixy V1");
            //options.RoutePrefix = string.Empty;
            options.RoutePrefix = "swagger";
        });
        //}
        app.UseHttpsRedirection();

        app.UseCors("DevelopmentPolicy");

        app.UseRouting();

        // Localization Middleware
        var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        // Seeders
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            await RoleSeeder.SeedAsync(roleManager);
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await UserSeeder.SeedAsync(userManager);
        }

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.MapHub<NotificationHub>("/hubs/notification");
    }
}