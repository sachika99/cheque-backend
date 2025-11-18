using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
    {
        public void Configure(EntityTypeBuilder<Cheque> b)
        {
            b.ToTable("Cheques");
            b.HasKey(x => x.Id);

            // Business identifiers
            b.Property(x => x.ChequeId)
                .HasMaxLength(50)     // GUID or external id
                .IsRequired();

            b.Property(x => x.ChequeNo)
                .HasMaxLength(40)
                .IsRequired();

            b.Property(x => x.ReceiptNo).HasMaxLength(50);
            b.Property(x => x.InvoiceNo).HasMaxLength(50);

            b.Property(x => x.InvoiceAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            b.Property(x => x.ChequeAmount).HasColumnType("decimal(18,2)").IsRequired();

            b.Property(x => x.PayeeName).HasMaxLength(200);
            b.Property(x => x.BankStatement).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            b.Property(x => x.CreatedBy).HasMaxLength(100);
            b.Property(x => x.ApprovedBy).HasMaxLength(100);

            // FK relationships
            b.HasOne(x => x.Vendor)
                .WithMany(x => x.Cheques)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ChequeBook)
                .WithMany(x => x.Cheques)
                .HasForeignKey(x => x.ChequeBookId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.BankAccount)
                .WithMany(x => x.Cheques)
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique cheque number within a bank account
            b.HasIndex(x => new { x.BankAccountId, x.ChequeNo }).IsUnique();

            // Fast filters / reporting
            b.HasIndex(x => x.DueDate);
            b.HasIndex(x => x.ChequeDate);
            b.HasIndex(x => x.Status);

            // IsOverdue is a domain-computed property, not stored in database
            b.Ignore(x => x.IsOverdue);
        }
    }
}
