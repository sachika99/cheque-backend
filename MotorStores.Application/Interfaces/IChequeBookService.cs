using MotorStores.Application.DTOs;

namespace MotorStores.Application.Interfaces
{
    public interface IChequeBookService
    {
        Task<IEnumerable<ChequeBookDto>> GetAllAsync();
        Task<IEnumerable<ChequeBookDto>> GetByBankAccountIdAsync(int bankAccountId);
        Task<ChequeBookDto?> GetByIdAsync(int id);
        Task<ChequeBookDto> CreateAsync(ChequeBookDto dto);
        Task<ChequeBookDto> UpdateAsync(ChequeBookDto dto);
        Task<bool> DeleteAsync(int id);
        Task<string> GetNextChequeNumberAsync(int chequeBookId);
        Task<ChequeBookDto> UpdateCurrentChequeNoAsync(int chequeBookId, int currentChequeNo);
    }
}
