using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Services
{
    public class ChequeService : IChequeService
    {
        private readonly ApplicationDbContext _context;

        public ChequeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChequeDto> CreateAsync(ChequeDto dto)
        {
            // Validate vendor exists
            var vendor = await _context.Set<Vendor>()
                .FirstOrDefaultAsync(v => v.Id == dto.SupplierId);

            if (vendor == null)
                throw new InvalidOperationException($"Vendor with ID {dto.SupplierId} not found.");

            // Validate bank account exists
            var bankAccount = await _context.BankAccounts
                .Include(ba => ba.Bank)
                .FirstOrDefaultAsync(ba => ba.Id == dto.BankAccountId);

            if (bankAccount == null)
                throw new InvalidOperationException($"Bank account with ID {dto.BankAccountId} not found.");

            // Auto-calculate DueDate
            DateTime? dueDate = null;
            if (dto.InvoiceDate.HasValue && vendor.CrediPeriodDays.HasValue)
            {
                dueDate = dto.InvoiceDate.Value.AddDays(vendor.CrediPeriodDays.Value);
            }

            var cheque = new Cheque
            {
                ChequeId = Guid.NewGuid().ToString(),
                VendorId = dto.SupplierId,
                BankAccountId = dto.BankAccountId,
                InvoiceNo = dto.InvoiceNo,
                InvoiceDate = dto.InvoiceDate,
                InvoiceAmount = dto.InvoiceAmount,
                ReceiptNo = dto.ReceiptNo,
                ChequeNo = dto.ChequeNo,
                ChequeDate = dto.ChequeDate,
                DueDate = dueDate,
                ChequeAmount = dto.ChequeAmount,
                PayeeName = dto.PayeeName,
                Status = ChequeStatus.Pending,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cheques.Add(cheque);

            // Create history entry for creation
            var history = new ChequeHistory
            {
                ChequeId = int.Parse(cheque.ChequeId),
                Action = "Created",
                OldStatus = null,
                NewStatus = ChequeStatus.Pending.ToString(),
                ChangedBy = "System",
                Remarks = "Cheque created",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChequeHistories.Add(history);
            await _context.SaveChangesAsync();

            return await MapToDto(cheque);
        }

        public async Task UpdateStatusAsync(string chequeId, string newStatus, string user)
        {
            var cheque = await _context.Cheques
                .FirstOrDefaultAsync(c => c.ChequeId == chequeId);

            if (cheque == null)
                throw new InvalidOperationException($"Cheque with ID {chequeId} not found.");

            // Validate status
            if (!Enum.TryParse<ChequeStatus>(newStatus, out var status))
                throw new ArgumentException($"Invalid status: {newStatus}");

            var oldStatus = cheque.Status.ToString();
            cheque.Status = status;
            cheque.UpdatedAt = DateTime.UtcNow;

            // Update specific dates based on status
            if (status == ChequeStatus.Issued)
            {
                cheque.IssueDate = DateTime.UtcNow;
            }
            else if (status == ChequeStatus.Cleared)
            {
                cheque.ClearedDate = DateTime.UtcNow;
            }

            // Create history entry
            var history = new ChequeHistory
            {
                ChequeId = int.Parse(cheque.ChequeId),
                Action = "Status Changed",
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = user,
                Remarks = $"Status changed from {oldStatus} to {newStatus}",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChequeHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChequeReportDto>> GetDueThisMonthAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var cheques = await _context.Cheques
                .Include(c => c.Vendor)
                .Include(c => c.BankAccount)
                .Where(c => c.DueDate.HasValue &&
                           c.DueDate.Value >= startOfMonth &&
                           c.DueDate.Value <= endOfMonth &&
                           c.Status != ChequeStatus.Cleared &&
                           c.Status != ChequeStatus.Cancelled)
                .OrderBy(c => c.DueDate)
                .ToListAsync();

            return cheques.Select(MapToReportDto);
        }

        public async Task<IEnumerable<ChequeReportDto>> GetOverdueChequesAsync()
        {
            var now = DateTime.UtcNow;

            var cheques = await _context.Cheques
                .Include(c => c.Vendor)
                .Include(c => c.BankAccount)
                .Where(c => c.DueDate.HasValue &&
                           c.DueDate.Value < now &&
                           c.Status != ChequeStatus.Cleared &&
                           c.Status != ChequeStatus.Cancelled)
                .OrderBy(c => c.DueDate)
                .ToListAsync();

            return cheques.Select(MapToReportDto);
        }

        public async Task<IEnumerable<ChequeReportDto>> GetClearedChequesAsync()
        {
            var cheques = await _context.Cheques
                .Include(c => c.Vendor)
                .Include(c => c.BankAccount)
                .Where(c => c.Status == ChequeStatus.Cleared)
                .OrderByDescending(c => c.ClearedDate)
                .ToListAsync();

            return cheques.Select(MapToReportDto);
        }

        public async Task<IEnumerable<ChequeReportDto>> GetAllChequesAsync(string? search = null)
        {
            var query = _context.Cheques
                .Include(c => c.Vendor)
                .Include(c => c.BankAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.ChequeNo.ToLower().Contains(search) ||
                    (c.InvoiceNo != null && c.InvoiceNo.ToLower().Contains(search)) ||
                    (c.Vendor != null && c.Vendor.VendorName.ToLower().Contains(search)));
            }

            var cheques = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return cheques.Select(MapToReportDto);
        }

        public async Task MarkAsVerifiedAsync(string chequeId)
        {
            var cheque = await _context.Cheques
                .FirstOrDefaultAsync(c => c.ChequeId == chequeId);

            if (cheque == null)
                throw new InvalidOperationException($"Cheque with ID {chequeId} not found.");

            cheque.IsVerified = true;
            cheque.UpdatedAt = DateTime.UtcNow;

            // Create history entry
            var history = new ChequeHistory
            {
                ChequeId = int.Parse(cheque.ChequeId),
                Action = "Verified",
                OldStatus = cheque.Status.ToString(),
                NewStatus = cheque.Status.ToString(),
                ChangedBy = "System",
                Remarks = "Cheque marked as verified after reconciliation",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChequeHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        private async Task<ChequeDto> MapToDto(Cheque cheque)
        {
            // Load navigation properties if not loaded
            if (cheque.Vendor == null)
            {
                await _context.Entry(cheque)
                    .Reference(c => c.Vendor)
                    .LoadAsync();
            }

            if (cheque.BankAccount == null)
            {
                await _context.Entry(cheque)
                    .Reference(c => c.BankAccount)
                    .LoadAsync();
            }

            return new ChequeDto
            {
                ChequeId = cheque.ChequeId,
                SupplierId = cheque.VendorId,
                SupplierName = cheque.Vendor?.VendorName ?? "Unknown",
                BankAccountId = cheque.BankAccountId,
                AccountNo = cheque.BankAccount?.AccountNo ?? "Unknown",
                InvoiceNo = cheque.InvoiceNo,
                InvoiceDate = cheque.InvoiceDate,
                InvoiceAmount = cheque.InvoiceAmount,
                ChequeNo = cheque.ChequeNo,
                ChequeDate = cheque.ChequeDate,
                DueDate = cheque.DueDate,
                ChequeAmount = cheque.ChequeAmount,
                ReceiptNo = cheque.ReceiptNo,
                PayeeName = cheque.PayeeName,
                Status = cheque.Status.ToString(),
                IsVerified = cheque.IsVerified,
                IsOverdue = cheque.IsOverdue
            };
        }

        private ChequeReportDto MapToReportDto(Cheque cheque)
        {
            return new ChequeReportDto
            {
                Vendor = cheque.Vendor?.VendorName ?? "Unknown",
                InvoiceNo = cheque.InvoiceNo ?? "N/A",
                ChequeNo = cheque.ChequeNo,
                Amount = cheque.ChequeAmount,
                DueDate = cheque.DueDate,
                IsOverdue = cheque.IsOverdue,
                AccountNo = cheque.BankAccount?.AccountNo ?? "Unknown",
                Status = cheque.Status.ToString()
            };
        }
    }
}
