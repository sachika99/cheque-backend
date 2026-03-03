// Application/Interfaces/IBankAccountRepository.cs
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;

public interface IBankAccountRepository : IRepository<BankAccount>
{
    Task<IEnumerable<BankAccount>> GetAllWithBankAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BankAccount>> GetByBankIdAsync(int bankId, CancellationToken cancellationToken = default);
    Task<BankAccount?> GetByIdWithBankAsync(int id, CancellationToken cancellationToken = default);
    Task<BankAccount?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AccountNoExistsAsync(string accountNo, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> BankExistsAsync(int bankId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BankAccount>> GetAllByBankAsync(CancellationToken cancellationToken = default);
}