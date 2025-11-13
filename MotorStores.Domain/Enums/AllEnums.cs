
namespace MotorStores.Domain.Enums
{
    public enum VendorStatus{ Active = 1, Inactive = 2, Suspended = 3,Blacklisted = 4 }
    public enum BankStatus { Active, Inactive }
    public enum AccountStatus { Active, Closed }
    public enum ChequeBookStatus { Active, Completed, Cancelled }
    public enum ChequeStatus { Pending, Issued, Cleared, Cancelled, Returned }
}
