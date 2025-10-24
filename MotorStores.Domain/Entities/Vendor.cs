using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities;
 
public class Vendor : AuditableEntity
{ 
    public string VendorCode { get; set; } = string.Empty;
 
    public string VendorName { get; set; } = string.Empty;
 
    public string? VendorAddress { get; set; }
 
    public string? VendorPhoneNo { get; set; }
 
    public string? VendorEmail { get; set; }
 
    public string? BankName { get; set; }
 
    public string? AccountNumber { get; set; }

    public int? CrediPeriodDays { get; set; }
 
    public VendorStatus Status { get; set; } = VendorStatus.Active;
 
    public string? Notes { get; set; }
 
    public string? ContactPerson { get; set; } 
   
    public bool CanReceivePayments()
    {
        return Status == VendorStatus.Active &&
               !string.IsNullOrEmpty(BankName) &&
               !string.IsNullOrEmpty(AccountNumber);
    } 
    
    public void Activate()
    {
        Status = VendorStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
 
    public void Deactivate()
    {
        Status = VendorStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}
