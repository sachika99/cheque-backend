using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities
{
    public class BankAccount : AuditableEntity
    {
        public int BankId { get; set; }
        public string AccountNo { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public decimal Balance { get; set; } = 0;
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public Bank? Bank { get; set; }
        public ICollection<ChequeBook> ChequeBooks { get; set; } = new List<ChequeBook>();
        public ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
    }
}
