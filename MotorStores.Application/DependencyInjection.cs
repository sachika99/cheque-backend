using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MotorStores.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());


        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // NOTE: Services are registered in Infrastructure layer DependencyInjection
        // because they depend on ApplicationDbContext

        // TODO: Add FluentValidation when validation is implemented
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

