// Infrastructure/Repositories/ChequeBookRepository.cs
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class ChequeBookRepository : Repository<ChequeBook>, IChequeBookRepository
{

    private readonly ICurrentUserService _currentUser;

    public ChequeBookRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context)
    {
        _currentUser = currentUser;
    }

    private IQueryable<ChequeBook> UserChequeBooks =>
        _dbSet.Where(c => c.UserId == _currentUser.UserId);
    public async Task<IEnumerable<ChequeBook>> GetAllWithAccountAsync(CancellationToken cancellationToken = default)
        => await UserChequeBooks
            .Include(cb => cb.BankAccount)
            .OrderByDescending(cb => cb.IssuedDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ChequeBook>> GetByBankAccountIdAsync(int bankAccountId, CancellationToken cancellationToken = default)
        => await UserChequeBooks
            .Include(cb => cb.BankAccount)
            .Where(cb => cb.BankAccountId == bankAccountId)
            .OrderByDescending(cb => cb.IssuedDate)
            .ToListAsync(cancellationToken);

    public async Task<ChequeBook?> GetByIdWithAccountAsync(int id, CancellationToken cancellationToken = default)
        => await UserChequeBooks
            .Include(cb => cb.BankAccount)
            .FirstOrDefaultAsync(cb => cb.Id == id, cancellationToken);

    public async Task<ChequeBook?> GetByIdWithChequesAsync(int id, CancellationToken cancellationToken = default)
        => await UserChequeBooks
            .Include(cb => cb.Cheques)
            .FirstOrDefaultAsync(cb => cb.Id == id, cancellationToken);

    public async Task<bool> ChequeBookNoExistsAsync(string chequeBookNo, int? excludeId = null, CancellationToken cancellationToken = default)
        => await UserChequeBooks
            .AnyAsync(cb => cb.ChequeBookNo == chequeBookNo &&
                (!excludeId.HasValue || cb.Id != excludeId.Value), cancellationToken);

    public async Task<bool> BankAccountExistsAsync(int bankAccountId, CancellationToken cancellationToken = default)
        => await _context.BankAccounts.AnyAsync(ba => ba.Id == bankAccountId, cancellationToken);
    public async Task<bool> UpdateCurrentChequeNoAsync(
    int chequeBookId,
    string currentChequeNo,
    CancellationToken cancellationToken = default)
    {
        var chequeBook = await UserChequeBooks
            .FirstOrDefaultAsync(cb => cb.Id == chequeBookId, cancellationToken);

        if (chequeBook == null)
            return false;

        chequeBook.CurrentChequeNo = currentChequeNo;
        chequeBook.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
