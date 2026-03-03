
namespace MotorStores.Application.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string? UserName { get; }
    string? CreatedBy { get; }
    string? Role { get; }
}