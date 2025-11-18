using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities
{
    public class Cheque : AuditableEntity
    {
        public string ChequeId { get; set; } = Guid.NewGuid().ToString();
        public int VendorId { get; set; }
        public int ChequeBookId { get; set; }
        public int BankAccountId { get; set; }

        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }

        public string? ReceiptNo { get; set; }
        public string ChequeNo { get; set; } = null!;
        public DateTime ChequeDate { get; set; }
        public DateTime? DueDate { get; set; }

        public decimal ChequeAmount { get; set; }
        public string? PayeeName { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.Pending;
        public DateTime? IssueDate { get; set; }
        public DateTime? ClearedDate { get; set; }

        public string? BankStatement { get; set; }
        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? PrintedAt { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != ChequeStatus.Cleared;

        public Vendor? Vendor { get; set; }
        public ChequeBook? ChequeBook { get; set; }
        public BankAccount? BankAccount { get; set; }
        public ICollection<ChequeHistory> ChequeHistories { get; set; } = new List<ChequeHistory>();
    }
}
