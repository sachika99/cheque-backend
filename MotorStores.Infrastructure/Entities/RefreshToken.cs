using MotorStores.Infrastructure.Entities;
using System.Security.Cryptography;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = default!;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; } = default!;
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string UserId { get; set; } = default!;
    public AppUser User { get; set; } = default!;
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => Revoked == null && !IsExpired;

    public static string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
