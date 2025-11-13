using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> b)
        {
            b.ToTable("BankAccounts");
            b.HasKey(x => x.Id);

            b.Property(x => x.AccountNo)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.AccountName)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.AccountType)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // FK relationship
            b.HasOne(x => x.Bank)
                .WithMany(x => x.BankAccounts)
                .HasForeignKey(x => x.BankId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint on account number
            b.HasIndex(x => x.AccountNo).IsUnique();
        }
    }
}
