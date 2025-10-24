using Microsoft.EntityFrameworkCore;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence;
 
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
 
    public DbSet<Vendor> Vendors => Set<Vendor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
         
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-57TB2HG\\SQLEXPRESS;Database=MotorStoresChequeDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;",
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
            );
        }
    }

}
