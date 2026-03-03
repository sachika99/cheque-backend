// Interfaces/IBankRepository.cs
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;

public interface IBankRepository : IRepository<Bank>
{
    Task<Bank?> GetByIdWithAccountsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> BranchCodeExistsAsync(string branchCode, CancellationToken cancellationToken = default, int? excludeId = null);
    Task<bool> BankAndBranchExistsAsync(string bankName, string branchName, CancellationToken cancellationToken = default);
}