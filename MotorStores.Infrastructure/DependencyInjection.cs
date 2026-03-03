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
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
            )
        );

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IBankRepository, BankRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IChequeBookRepository ,ChequeBookRepository>();
        services.AddScoped<IChequeRepository, ChequeRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUserIdRepository, UserIdRepository>();

        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
