// Infrastructure/Repositories/BankAccountRepository.cs
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository
{
    private readonly ICurrentUserService _currentUser;

    public BankAccountRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context)
    {
        _currentUser = currentUser;
    }
    private IQueryable<BankAccount> UserBankAccounts =>
        _dbSet.Where(c => c.UserId == _currentUser.UserId);
    public async Task<IEnumerable<BankAccount>> GetAllWithBankAsync(CancellationToken cancellationToken = default)
        => await UserBankAccounts
            .Include(ba => ba.Bank)
            .OrderBy(ba => ba.AccountNo)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<BankAccount>> GetByBankIdAsync(int bankId, CancellationToken cancellationToken = default)
        => await UserBankAccounts
            .Include(ba => ba.Bank)
            .Where(ba => ba.BankId == bankId)
            .OrderBy(ba => ba.AccountNo)
            .ToListAsync(cancellationToken);

    public async Task<BankAccount?> GetByIdWithBankAsync(int id, CancellationToken cancellationToken = default)
        => await UserBankAccounts
            .Include(ba => ba.Bank)
            .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

    public async Task<BankAccount?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await UserBankAccounts
            .Include(ba => ba.Bank)
            .Include(ba => ba.ChequeBooks)
            .Include(ba => ba.Cheques)
            .FirstOrDefaultAsync(ba => ba.Id == id, cancellationToken);

    public async Task<bool> AccountNoExistsAsync(string accountNo, int? excludeId = null, CancellationToken cancellationToken = default)
        => await UserBankAccounts
            .AnyAsync(ba => ba.AccountNo == accountNo &&
                (!excludeId.HasValue || ba.Id != excludeId.Value), cancellationToken);

    public async Task<bool> BankExistsAsync(int bankId, CancellationToken cancellationToken = default)
        => await _context.Banks.AnyAsync(b => b.Id == bankId, cancellationToken);

    public async Task<IEnumerable<BankAccount>> GetAllByBankAsync(CancellationToken cancellationToken = default)
        => await UserBankAccounts.ToListAsync(cancellationToken);
}