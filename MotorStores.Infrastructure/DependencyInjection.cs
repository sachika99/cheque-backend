using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotorStores.Application.Interfaces;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Repositories;
using MotorStores.Infrastructure.Services;

namespace MotorStores.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
            ));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IVendorRepository, VendorRepository>();

        services.AddScoped<IChequeService, ChequeService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<IChequeBookService, ChequeBookService>();

        return services;
    }
}

