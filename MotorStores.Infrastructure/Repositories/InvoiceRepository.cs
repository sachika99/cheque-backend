// Infrastructure/Repositories/InvoiceRepository.cs
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    private readonly ICurrentUserService _currentUser;
    public InvoiceRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context)
    {
        _currentUser = currentUser;
    }

    private IQueryable<Invoice> UserIvoice =>
        _dbSet.Where(c => c.UserId == _currentUser.UserId);
    public async Task<IEnumerable<Invoice>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
        => await UserIvoice
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
}