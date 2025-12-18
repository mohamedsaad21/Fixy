using Fixy.Infrastructure.InfrastructureBases;
using Microsoft.Extensions.DependencyInjection;

namespace Fixy.Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        return services;
    }
}
