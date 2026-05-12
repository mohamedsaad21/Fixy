using Fixy.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fixy.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        var healthChecks = services.AddHealthChecks();
        healthChecks.AddCheck("Application", () => HealthCheckResult.Healthy());

        healthChecks.AddCheck<DatabaseHealthCheck>(
            name: "Database",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["ready", "critical", "sql"]
        );

        healthChecks.AddCheck<RedisHealthCheck>(
                name: "Redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["cache", "redis", "optional"]
            );

        return services;

    }
}
