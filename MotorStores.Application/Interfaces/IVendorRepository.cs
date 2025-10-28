using MotorStores.Domain.Entities;

namespace MotorStores.Application.Interfaces;
 
public interface IVendorRepository : IRepository<Vendor>
{ 
    Task<Vendor?> GetByVendorCodeAsync(string vendorCode, CancellationToken cancellationToken = default);
 
    Task<IEnumerable<Vendor>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
 
    Task<IEnumerable<Vendor>> GetActiveVendorsAsync(CancellationToken cancellationToken = default);
 
    Task<bool> VendorCodeExistsAsync(string vendorCode, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<Vendor?> GetLastVendorAsync(CancellationToken cancellationToken = default);
}
