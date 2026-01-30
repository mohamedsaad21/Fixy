using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Fixy.Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
