using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Repositories;

public class ChequeRepository : Repository<Cheque>, IChequeRepository
{

    private readonly ICurrentUserService _currentUser;

    public ChequeRepository(ApplicationDbContext context, ICurrentUserService currentUser)
        : base(context)
    {
        _currentUser = currentUser;
    }

    private IQueryable<Cheque> UserCheques =>
        _dbSet.Where(c => c.UserId == _currentUser.UserId);

    public override async Task<IEnumerable<Cheque>> GetAllAsync(CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Invoices)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cheque>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Include(c => c.Invoices)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Cheque?> GetByChequeIdAsync(string chequeId, CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.ChequeId == chequeId, cancellationToken);

    public async Task<Cheque?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Cheque?> GetByChequeIdWithDetailsAsync(string chequeId, CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.ChequeId == chequeId, cancellationToken);

    public async Task<IEnumerable<Cheque>> GetDueThisMonthAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        return await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Where(c => c.DueDate >= start && c.DueDate <= end
                     && c.Status != ChequeStatus.Cleared
                     && c.Status != ChequeStatus.Cancelled)
            .OrderBy(c => c.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cheque>> GetOverdueAsync(CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Where(c => c.DueDate < DateTime.UtcNow
                     && c.Status != ChequeStatus.Cleared
                     && c.Status != ChequeStatus.Cancelled)
            .OrderBy(c => c.DueDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cheque>> GetClearedAsync(CancellationToken cancellationToken = default)
        => await UserCheques
            .Include(c => c.Vendor)
            .Include(c => c.BankAccount)
            .Where(c => c.Status == ChequeStatus.Cleared)
            .OrderByDescending(c => c.ClearedDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cheque>> GetByIdsAsync(List<string> chequeIds, CancellationToken cancellationToken = default)
        => await UserCheques
            .Where(c => chequeIds.Contains(c.ChequeId))
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountAsync(
        int bankAccountId, CancellationToken cancellationToken = default)
        => await UserCheques
            .Where(c => c.BankAccountId == bankAccountId)
            .GroupBy(c => c.Status)
            .Select(g => new ChequeStatusSummaryDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                TotalAmount = g.Sum(c => c.ChequeAmount)
            })
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountTimeAsync(
        int bankAccountId, DateTime? startDate, DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        var query = UserCheques.Where(c => c.BankAccountId == bankAccountId);

        if (startDate.HasValue) query = query.Where(c => c.ChequeDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.ChequeDate <= endDate.Value);

        return await query
            .GroupBy(c => c.Status)
            .Select(g => new ChequeStatusSummaryDto
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                TotalAmount = g.Sum(c => c.ChequeAmount)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(ChequeHistory history, CancellationToken cancellationToken = default)
        => await _context.ChequeHistories.AddAsync(history, cancellationToken);

    public async Task DeleteHistoriesByChequeAsync(int chequeId, CancellationToken cancellationToken = default)
    {
        var histories = await _context.ChequeHistories
            .Where(h => h.ChequeId == chequeId)
            .ToListAsync(cancellationToken);
        _context.ChequeHistories.RemoveRange(histories);
    }

    public Task RemoveInvoicesAsync(IEnumerable<Invoice> invoices)
    {
        _context.Invoices.RemoveRange(invoices);
        return Task.CompletedTask;
    }

    public async Task AddInvoiceAsync(Invoice invoice)
        => await _context.Invoices.AddAsync(invoice);

    public async Task<decimal> GetCurrentMonthTotalByAccountAsync(
    int bankAccountId, DateTime? startDate, DateTime? endDate,
    CancellationToken cancellationToken = default)
    {
        var query = UserCheques.Where(c => c.BankAccountId == bankAccountId);

        if (startDate.HasValue) query = query.Where(c => c.DueDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(c => c.DueDate <= endDate.Value);

        return await query.SumAsync(c => c.ChequeAmount, cancellationToken);
    }
}