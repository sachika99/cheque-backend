namespace MotorStores.Domain.Common;
 
public abstract class AuditableEntity : BaseEntity
{
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

