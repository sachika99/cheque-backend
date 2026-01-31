using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces
{
    public interface IChequeRepository : IRepository<Cheque>
    {
        Task<IEnumerable<Cheque>> GetAllAsync(CancellationToken cancellationToken);
        //Task<ChequeDto> CreateAsync(ChequeDto dto);
        //Task UpdateStatusAsync(string chequeId, string newStatus, string user);
        //Task<IEnumerable<ChequeReportDto>> GetDueThisMonthAsync();
        //Task<ChequeReportDto?> GetByIdAsync(string id);
        //Task<IEnumerable<ChequeReportDto>> GetOverdueChequesAsync();
        //Task<IEnumerable<ChequeReportDto>> GetClearedChequesAsync();
        //Task<IEnumerable<ChequeReportDto>> GetAllChequesAsync(string? search = null);
        //Task MarkAsVerifiedAsync(string chequeId);
    }
}
