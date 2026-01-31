using MotorStores.Application.DTOs;

namespace MotorStores.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllAsync();
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<InvoiceDto> CreateAsync(InvoiceDto dto);
        Task<InvoiceDto> UpdateAsync(InvoiceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
