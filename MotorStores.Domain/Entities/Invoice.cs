using MotorStores.Domain.Common;

namespace MotorStores.Domain.Entities
{
    public class Invoice : AuditableEntity
    {
        public int? ChequeId { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal? InvoiceAmount { get; set; }

        public Cheque? Cheque { get; set; }
    }
}
