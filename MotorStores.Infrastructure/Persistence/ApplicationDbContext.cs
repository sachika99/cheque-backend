using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MotorStores.Domain.Entities;    
using MotorStores.Infrastructure.Entities; 
using System;
using System.IO;

namespace MotorStores.Infrastructure.Persistence
{

    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Bank> Banks => Set<Bank>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<ChequeBook> ChequeBooks => Set<ChequeBook>();
        public DbSet<Cheque> Cheques => Set<Cheque>();
        public DbSet<ChequeHistory> ChequeHistories => Set<ChequeHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        if (!optionsBuilder.IsConfigured)
        { 
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MotorStores.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();
            
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }
            
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
            );
        }
    }
    }
}
