using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MotorStores.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        optionsBuilder.UseSqlServer(
          "Server=DESKTOP-57TB2HG\\SQLEXPRESS;Database=MotorStoresChequeDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;",
            sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
        );
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
