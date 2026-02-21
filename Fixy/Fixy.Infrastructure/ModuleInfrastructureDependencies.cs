using Fixy.Application.Abstracts;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Persistence.Repositories;
using Fixy.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fixy.Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddTransient<IPaymobService, PaymobService>();
        services.AddHttpClient<IPaymobService, PaymobService>();
        
        return services;
    }
}
