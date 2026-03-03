using MotorStores.Domain.Common;
using MotorStores.Domain.Enums;

namespace MotorStores.Domain.Entities;
 
public class UserId : AuditableEntity
{
    public string? Role { get; set; } = null!;
}
