using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class ChequeBookConfiguration : IEntityTypeConfiguration<ChequeBook>
    {
        public void Configure(EntityTypeBuilder<ChequeBook> b)
        {
            b.ToTable("Cheque_Books");
            b.HasKey(x => x.Id);

            b.Property(x => x.ChequeBookNo).HasMaxLength(50).IsRequired();
            b.Property(x => x.StartChequeNo).IsRequired();
            b.Property(x => x.EndChequeNo).IsRequired();
            b.Property(x => x.CurrentChequeNo).IsRequired();

            b.Property(x => x.IssuedDate).IsRequired();

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Unique cheque book per bank account
            b.HasIndex(x => new { x.BankAccountId, x.ChequeBookNo }).IsUnique();
        }
    }
}
