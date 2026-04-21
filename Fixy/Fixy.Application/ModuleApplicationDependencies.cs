using Fixy.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fixy.Application;

public static class ModuleApplicationDependencies
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // Add MediatR
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // Add Fluent Validation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // Add AutoMapper
        services.AddAutoMapper(mapper => { }, Assembly.GetExecutingAssembly());  
        return services;
    }
}
