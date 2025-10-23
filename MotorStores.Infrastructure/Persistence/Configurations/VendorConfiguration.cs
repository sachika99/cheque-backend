using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Infrastructure.Persistence.Configurations;
 
public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    { 
        builder.ToTable("Vendors");
 
        builder.HasKey(v => v.Id);
 
        builder.Property(v => v.VendorCode)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Unique vendor code identifier");

        builder.Property(v => v.VendorName)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Full name of the vendor/supplier");

        builder.Property(v => v.VendorAddress)
            .HasMaxLength(500)
            .HasComment("Physical address of the vendor");

        builder.Property(v => v.VendorPhoneNo)
            .HasMaxLength(20)
            .HasComment("Contact phone number");

        builder.Property(v => v.VendorEmail)
            .HasMaxLength(100)
            .HasComment("Contact email address");

        builder.Property(v => v.BankName)
            .HasMaxLength(100)
            .HasComment("Bank name for payment processing");

        builder.Property(v => v.AccountNumber)
            .HasMaxLength(50)
            .HasComment("Bank account number for payments");

        builder.Property(v => v.CrediPeriodDays)
            .HasComment("Credit period in days");

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Current status of the vendor");

        builder.Property(v => v.Notes)
            .HasMaxLength(1000)
            .HasComment("Additional notes about the vendor");

        builder.Property(v => v.ContactPerson)
            .HasMaxLength(255)
            .HasComment("Contact person name");
 
        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Record creation timestamp");

        builder.Property(v => v.UpdatedAt)
            .HasComment("Record last update timestamp");

        builder.Property(v => v.CreatedBy)
            .HasMaxLength(100)
            .HasComment("User who created the record");

        builder.Property(v => v.UpdatedBy)
            .HasMaxLength(100)
            .HasComment("User who last updated the record");
 
        builder.HasIndex(v => v.VendorCode)
            .IsUnique()
            .HasDatabaseName("IX_Vendors_VendorCode");

        builder.HasIndex(v => v.VendorName)
            .HasDatabaseName("IX_Vendors_VendorName");

        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_Vendors_Status");

        builder.HasIndex(v => v.VendorEmail)
            .HasDatabaseName("IX_Vendors_VendorEmail");
 
    }
}
