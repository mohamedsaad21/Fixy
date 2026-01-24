using Fixy.Infrastructure.InfrastructureBases;
using Fixy.Infrastructure.Persistence;
using Fixy.Infrastructure.Persistence.Abstracts;
using Fixy.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Fixy.Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<IServiceCategoryReadRepository, ServiceCategoryReadRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IServiceRequestReadRepository, ServiceRequestReadRepository>();
        return services;
    }
}
