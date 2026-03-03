// Application/Interfaces/IChequeRepository.cs
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Interfaces;

public interface IChequeRepository : IRepository<Cheque>
{
    Task<Cheque?> GetByChequeIdAsync(string chequeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cheque>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Cheque?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Cheque?> GetByChequeIdWithDetailsAsync(string chequeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cheque>> GetDueThisMonthAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cheque>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cheque>> GetClearedAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cheque>> GetByIdsAsync(List<string> chequeIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountAsync(int bankAccountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountTimeAsync(int bankAccountId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(ChequeHistory history, CancellationToken cancellationToken = default);
    Task DeleteHistoriesByChequeAsync(int chequeId, CancellationToken cancellationToken = default);
    Task RemoveInvoicesAsync(IEnumerable<Invoice> invoices);
    Task AddInvoiceAsync(Invoice invoice);
    Task<decimal> GetCurrentMonthTotalByAccountAsync(int bankAccountId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);

}