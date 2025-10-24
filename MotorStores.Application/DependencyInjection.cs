using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MotorStores.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // TODO: Add FluentValidation when validation is implemented
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}

