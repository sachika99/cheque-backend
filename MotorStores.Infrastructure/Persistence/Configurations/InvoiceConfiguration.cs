using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> b)
        {
            b.ToTable("Invoices");

            b.HasKey(x => x.Id);

            b.Property(x => x.InvoiceNo)
                .HasMaxLength(50)
                .IsRequired(false);

            b.Property(x => x.InvoiceAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            b.HasOne(x => x.Cheque)
                .WithMany(c => c.Invoices)
                .HasForeignKey(x => x.ChequeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
