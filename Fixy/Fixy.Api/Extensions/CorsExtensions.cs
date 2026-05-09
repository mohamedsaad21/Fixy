namespace Fixy.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:4200",
                        "http://127.0.0.1:5500"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        return services;
    }
}
