using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;

namespace MotorStores.Infrastructure.Persistence.Configurations
{
    public class ChequeHistoryConfiguration : IEntityTypeConfiguration<ChequeHistory>
    {
        public void Configure(EntityTypeBuilder<ChequeHistory> b)
        {
            b.ToTable("Cheque_History");
            b.HasKey(x => x.Id);

            b.Property(x => x.Action).HasMaxLength(80).IsRequired();
            b.Property(x => x.OldStatus).HasMaxLength(20);
            b.Property(x => x.NewStatus).HasMaxLength(20);
            b.Property(x => x.ChangedBy).HasMaxLength(100);
            b.Property(x => x.Remarks).HasMaxLength(500);

            b.HasOne(x => x.Cheque)
                .WithMany(x => x.ChequeHistories)
                .HasForeignKey(x => x.ChequeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ChequeId, x.CreatedAt });
        }
    }
}
