using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;
 
public class VendorRepository : Repository<Vendor>, IVendorRepository
{
    private readonly ICurrentUserService _currentUser;
    public VendorRepository(ApplicationDbContext context, ICurrentUserService currentUser) : base(context)
    {
        _currentUser = currentUser;
    }
    private IQueryable<Vendor> UserVendors =>
    _dbSet.Where(c => c.UserId == _currentUser.UserId);

    public override async Task<IEnumerable<Vendor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
       return await UserVendors
         .OrderByDescending(c => c.CreatedAt)
         .ToListAsync(cancellationToken);
    }
    public async Task<Vendor?> GetByVendorCodeAsync(string vendorCode, CancellationToken cancellationToken = default)
    {
        return await UserVendors
            .FirstOrDefaultAsync(v => v.VendorCode == vendorCode, cancellationToken);
    }

    public async Task<IEnumerable<Vendor>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await UserVendors
            .Where(v => v.VendorName.Contains(searchTerm))
            .OrderBy(v => v.VendorName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Vendor>> GetActiveVendorsAsync(CancellationToken cancellationToken = default)
    {
        return await UserVendors
            .Where(v => v.Status == VendorStatus.Active)
            .OrderBy(v => v.VendorName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VendorCodeExistsAsync(string vendorCode, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = UserVendors.Where(v => v.VendorCode == vendorCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Vendor?> GetLastVendorAsync(CancellationToken cancellationToken = default)
    {
        return await UserVendors
            .OrderByDescending(v => v.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
