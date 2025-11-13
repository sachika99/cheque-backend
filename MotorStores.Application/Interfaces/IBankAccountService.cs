using MotorStores.Application.DTOs;

namespace MotorStores.Application.Interfaces
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccountDto>> GetAllAsync();
        Task<IEnumerable<BankAccountDto>> GetByBankIdAsync(int bankId);
        Task<BankAccountDto?> GetByIdAsync(int id);
        Task<BankAccountDto> CreateAsync(BankAccountDto dto);
        Task<BankAccountDto> UpdateAsync(BankAccountDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
