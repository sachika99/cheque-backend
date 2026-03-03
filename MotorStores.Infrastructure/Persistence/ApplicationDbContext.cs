using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Entities;
using System;

namespace MotorStores.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Bank> Banks => Set<Bank>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<ChequeBook> ChequeBooks => Set<ChequeBook>();
        public DbSet<Cheque> Cheques => Set<Cheque>();
        public DbSet<ChequeHistory> ChequeHistories => Set<ChequeHistory>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<UserId> UserIds => Set<UserId>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue
                    ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime())
                    : v,
                v => v.HasValue
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}