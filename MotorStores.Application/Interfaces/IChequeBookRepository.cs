// Application/Interfaces/IChequeBookRepository.cs
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;

public interface IChequeBookRepository : IRepository<ChequeBook>
{
    Task<IEnumerable<ChequeBook>> GetAllWithAccountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ChequeBook>> GetByBankAccountIdAsync(int bankAccountId, CancellationToken cancellationToken = default);
    Task<ChequeBook?> GetByIdWithAccountAsync(int id, CancellationToken cancellationToken = default);
    Task<ChequeBook?> GetByIdWithChequesAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ChequeBookNoExistsAsync(string chequeBookNo, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> BankAccountExistsAsync(int bankAccountId, CancellationToken cancellationToken = default);
    Task<bool> UpdateCurrentChequeNoAsync(int chequeBookId,string currentChequeNo,CancellationToken cancellationToken = default);

}