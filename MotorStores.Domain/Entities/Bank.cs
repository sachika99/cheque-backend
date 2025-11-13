using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities
{
    public class Bank : AuditableEntity
    {
        public string BankName { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public BankStatus Status { get; set; } = BankStatus.Active;

        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    }
}
