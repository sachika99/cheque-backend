using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities
{
    public class ChequeBook : AuditableEntity
    {
        public int BankAccountId { get; set; }
        public string ChequeBookNo { get; set; } = null!;
        public int StartChequeNo { get; set; }
        public int EndChequeNo { get; set; }
        public int CurrentChequeNo { get; set; }
        public ChequeBookStatus Status { get; set; } = ChequeBookStatus.Active;
        public DateTime IssuedDate { get; set; }

        public BankAccount? BankAccount { get; set; }
        public ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
    }
}
