// Application/Interfaces/IInvoiceRepository.cs
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}