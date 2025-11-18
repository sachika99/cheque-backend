using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorStores.Application.DTOs
{
    public class ChequeDto
    {
        public string ChequeId { get; set; } = null!;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public int BankAccountId { get; set; }
        public string AccountNo { get; set; } = null!;
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string ChequeNo { get; set; } = null!;
        public DateTime ChequeDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal ChequeAmount { get; set; }
        public string? ReceiptNo { get; set; }
        public string? PayeeName { get; set; }
        public string Status { get; set; } = "Pending";
        public bool IsVerified { get; set; }
        public bool IsOverdue { get; set; }
    }
}
