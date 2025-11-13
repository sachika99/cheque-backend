using MotorStores.Domain.Common;

namespace MotorStores.Domain.Entities
{
    public class ChequeHistory : AuditableEntity
    {
        public int ChequeId { get; set; }
        public string Action { get; set; } = null!;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? ChangedBy { get; set; }
        public string? Remarks { get; set; }

        public Cheque? Cheque { get; set; }
    }
}
