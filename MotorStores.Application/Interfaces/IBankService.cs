using MotorStores.Application.DTOs;

namespace MotorStores.Application.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<BankDto>> GetAllAsync();
        Task<BankDto?> GetByIdAsync(int id);
        Task<BankDto> CreateAsync(BankDto dto);
        Task<BankDto> UpdateAsync(BankDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
