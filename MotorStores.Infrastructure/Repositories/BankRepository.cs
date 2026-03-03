// Infrastructure/Repositories/BankRepository.cs
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class BankRepository : Repository<Bank>, IBankRepository
{
   
    private readonly ICurrentUserService _currentUser;

    public BankRepository(ApplicationDbContext context, ICurrentUserService currentUser) : base(context)
    {
        _currentUser = currentUser;
    }
    private IQueryable<Bank> UserBanks =>
        _dbSet.Where(c => c.UserId == _currentUser.UserId);
    public override async Task<IEnumerable<Bank>> GetAllAsync(CancellationToken cancellationToken = default)
        => await UserBanks
            .OrderBy(b => b.BankName)
            .ToListAsync(cancellationToken);

    public async Task<Bank?> GetByIdWithAccountsAsync(int id, CancellationToken cancellationToken = default)
        => await UserBanks
            .Include(b => b.BankAccounts)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<bool> BranchCodeExistsAsync(string branchCode, CancellationToken cancellationToken = default, int? excludeId = null)
        => await UserBanks
            .AnyAsync(b => b.BranchCode == branchCode && (!excludeId.HasValue || b.Id != excludeId.Value), cancellationToken);

    public async Task<bool> BankAndBranchExistsAsync(string bankName, string branchName, CancellationToken cancellationToken = default)
        => await UserBanks
            .AnyAsync(b => b.BankName == bankName && b.BranchName == branchName, cancellationToken);
}