using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;

public interface IUserIdRepository : IRepository<UserId>
{
    Task<UserId?> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserId?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserId>> GetAllAsync(CancellationToken cancellationToken = default); // ✅ Add this
    Task<UserId> UpdateAsync(UserId entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}