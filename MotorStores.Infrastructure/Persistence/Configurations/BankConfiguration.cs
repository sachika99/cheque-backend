using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> b)
        {
            b.ToTable("Banks");
            b.HasKey(x => x.Id);

            b.Property(x => x.BankName)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.BranchName)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.BranchCode)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Unique constraint on branch code
            b.HasIndex(x => x.BranchCode).IsUnique();
        }
    }
}
