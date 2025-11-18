using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorStores.Application.DTOs
{
    public class ChequeReportDto
    {
        public string Vendor { get; set; } = null!;
        public string InvoiceNo { get; set; } = null!;
        public string ChequeNo { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public string AccountNo { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
