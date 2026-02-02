using MotorStores.Application.DTOs;

namespace MotorStores.Application.Interfaces
{
    public interface IChequeService
    {
        Task<ChequeDto> CreateAsync(ChequeDto dto);
        Task UpdateStatusAsync(string chequeId, string newStatus, string user);
        Task UpdateStatusBulkAsync(
      List<string> chequeIds,
      string newStatus,
      string user
  );
        Task<IEnumerable<ChequeReportDto>> GetDueThisMonthAsync();
        Task<ChequeReportDto?> GetByIdAsync(string id);
        Task<IEnumerable<ChequeReportDto>> GetOverdueChequesAsync();
        Task<IEnumerable<ChequeReportDto>> GetClearedChequesAsync();
        Task<IEnumerable<ChequeReportDto>> GetAllChequesAsync(string? search = null);
        Task MarkAsVerifiedAsync(string chequeId);
        Task UpdateChequeAsync(string chequeId, UpdateChequeRequest request);
        Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountAsync(int bankAccountId);
        Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountTimeAsync(int bankAccountId, DateTime? startDate = null, DateTime? endDate = null);
        Task DeleteChequeAsync(int chequeId);


    }
}
