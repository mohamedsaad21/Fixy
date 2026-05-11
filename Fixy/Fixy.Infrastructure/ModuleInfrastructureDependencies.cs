using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Fixy.Infrastructure.ExternalServices;
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
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IPaymentService, StripeService>();
        services.AddTransient<IBookingService, BookingService>();
        services.AddTransient<IFeedbackService, FeedbackService>();
        services.AddTransient<IRatingService, RatingService>();

        services.AddSingleton<IStorageService, BlobStorageService>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IPresenceService, PresenceService>();

        return services;
    }
}
