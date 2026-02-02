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
        private readonly IChequeBookService _chequeBookService;

        public ChequeService(ApplicationDbContext context, IChequeBookService chequeBookService)
        {
            _context = context;
            _chequeBookService = chequeBookService;

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
                Id = 0,
                ChequeId = dto.ChequeId,
                VendorId = dto.SupplierId,
                ChequeBookId = dto.ChequeBookId,
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
            //var history = new ChequeHistory
            //{
            //    ChequeId = int.Parse(cheque.ChequeId),
            //    Action = "Created",
            //    OldStatus = null,
            //    NewStatus = ChequeStatus.Pending.ToString(),
            //    ChangedBy = "System",
            //    Remarks = "Cheque created",
            //    CreatedAt = DateTime.UtcNow
            //};

            //_context.ChequeHistories.Add(history);
            await _context.SaveChangesAsync();

            foreach (var inv in dto.Invoices)
            {
                var invoice = new Invoice
                {
                    Id = 0,
                    ChequeId = cheque.Id,
                    InvoiceNo = inv.InvoiceNo,
                    InvoiceAmount = inv.InvoiceAmount
                };

                _context.Invoices.Add(invoice);
            }

            await _context.SaveChangesAsync();
            await _chequeBookService.UpdateCurrentChequeNoAsync(cheque.ChequeBookId, int.Parse(cheque.ChequeNo));

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
                ChequeId = cheque.Id,
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
        public async Task UpdateStatusBulkAsync(
    List<string> chequeIds,
    string newStatus,
    string user)
        {
            if (chequeIds == null || !chequeIds.Any())
                throw new ArgumentException("ChequeIds cannot be empty.");

            var cheques = await _context.Cheques
                .Where(c => chequeIds.Contains(c.ChequeId))
                .ToListAsync();

            if (cheques.Count != chequeIds.Count)
                throw new InvalidOperationException("One or more cheques not found.");

            foreach (var cheque in cheques)
            {
                await UpdateChequeInternalAsync(cheque, newStatus, user);
            }

            await _context.SaveChangesAsync();
        }
        public async Task UpdateChequeAsync(string chequeId, UpdateChequeRequest request)
        {
            var cheque = await _context.Cheques
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.ChequeId == chequeId);

            if (cheque == null)
                throw new InvalidOperationException($"Cheque with ID {chequeId} not found.");

            // ================= UPDATE MAIN CHEQUE =================
            cheque.ChequeBookId = request.ChequeBookId;
            cheque.BankAccountId = request.BankAccountId;
            cheque.InvoiceDate = request.InvoiceDate;
            cheque.InvoiceAmount = request.InvoiceAmount;
            cheque.ChequeNo = request.ChequeNo;
            cheque.ChequeDate = request.ChequeDate;
            cheque.DueDate = request.DueDate;
            cheque.ChequeAmount = request.ChequeAmount;
            cheque.PayeeName = request.PayeeName;

            cheque.UpdatedAt = DateTime.UtcNow;

            // ================= SYNC INVOICES (UPDATE / INSERT / DELETE) =================

            // 1️⃣ DELETE removed invoices
            var incomingIds = request.Invoices
                .Where(i => i.Id > 0)
                .Select(i => i.Id)
                .ToList();

            var invoicesToDelete = cheque.Invoices
                .Where(i => !incomingIds.Contains(i.Id))
                .ToList();

            _context.Invoices.RemoveRange(invoicesToDelete);

            // 2️⃣ UPDATE existing & INSERT new invoices
            foreach (var inv in request.Invoices)
            {
                var existingInvoice = cheque.Invoices
                    .FirstOrDefault(i =>
                        (inv.Id > 0 && i.Id == inv.Id) ||
                        (!string.IsNullOrWhiteSpace(inv.InvoiceNo) && i.InvoiceNo == inv.InvoiceNo)
                    );

                if (existingInvoice != null)
                {
                    // UPDATE
                    existingInvoice.InvoiceNo = inv.InvoiceNo;
                    existingInvoice.InvoiceAmount = inv.InvoiceAmount;
                    existingInvoice.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // INSERT
                    _context.Invoices.Add(new Invoice
                    {
                        ChequeId = cheque.Id,
                        InvoiceNo = inv.InvoiceNo,
                        InvoiceAmount = inv.InvoiceAmount,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateChequeInternalAsync(
            Cheque cheque,
            string newStatus,
            string user)
        {
            if (!Enum.TryParse<ChequeStatus>(newStatus, out var status))
                throw new ArgumentException($"Invalid status: {newStatus}");

            var oldStatus = cheque.Status.ToString();

            if (oldStatus == newStatus)
                return; // no change

            cheque.Status = status;
            cheque.UpdatedAt = DateTime.UtcNow;

            if (status == ChequeStatus.Issued)
                cheque.IssueDate = DateTime.UtcNow;

            if (status == ChequeStatus.Cleared)
                cheque.ClearedDate = DateTime.UtcNow;

            var history = new ChequeHistory
            {
                ChequeId = cheque.Id,
                Action = "Status Changed",
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = user,
                Remarks = $"Status changed from {oldStatus} to {newStatus}",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChequeHistories.Add(history);
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
        public async Task<ChequeReportDto?> GetByIdAsync(string id)
        {
            var cheque = await _context.Cheques
                .Include(c => c.Vendor)
                .Include(c => c.BankAccount)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.ChequeId == id);

            if (cheque == null)
                return null;

            return MapToReportDto(cheque);
        }

        public async Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountAsync(int bankAccountId)
        {
            var summary = await _context.Cheques
                .Where(c => c.BankAccountId == bankAccountId)
                .GroupBy(c => c.Status)
                .Select(g => new ChequeStatusSummaryDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    TotalAmount = g.Sum(c => c.ChequeAmount)
                })
                .ToListAsync();

            return summary;
        }

        public async Task<IEnumerable<ChequeStatusSummaryDto>> GetStatusSummaryByBankAccountTimeAsync(int bankAccountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Cheques
                .Where(c => c.BankAccountId == bankAccountId);

            // Apply date filters if provided
            if (startDate.HasValue)
            {
                query = query.Where(c => c.ChequeDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.ChequeDate <= endDate.Value);
            }

            var summary = await query
                .GroupBy(c => c.Status)
                .Select(g => new ChequeStatusSummaryDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    TotalAmount = g.Sum(c => c.ChequeAmount)
                })
                .ToListAsync();

            return summary;
        }
        public async Task DeleteChequeAsync(int chequeId)
        {
            var cheque = await _context.Cheques
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == chequeId);

            if (cheque == null)
                throw new InvalidOperationException($"Cheque with ID {chequeId} not found.");

            // 1️⃣ Delete related invoices first
            if (cheque.Invoices.Any())
            {
                _context.Invoices.RemoveRange(cheque.Invoices);
            }

            // 2️⃣ Optional: delete cheque history
            var histories = await _context.ChequeHistories
                .Where(h => h.ChequeId == cheque.Id)
                .ToListAsync();

            if (histories.Any())
            {
                _context.ChequeHistories.RemoveRange(histories);
            }

            // 3️⃣ Delete cheque
            _context.Cheques.Remove(cheque);

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
                ChequeBookId = cheque.ChequeBookId,
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
               Id = cheque.Id,
                ChequeId = cheque.ChequeId,
                SupplierId = cheque.VendorId,
                SupplierName = cheque.Vendor?.VendorName ?? "Unknown",
                BankAccountId = cheque.BankAccountId,
                AccountNo = cheque.BankAccount?.AccountNo ?? "Unknown",
                ChequeBookId = cheque.ChequeBookId,
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
                IsOverdue = cheque.IsOverdue,
                 Invoices = cheque.Invoices.Select(i => new InvoiceDto
                 {
                     Id = i.Id,
                     InvoiceNo = i.InvoiceNo,
                     InvoiceAmount = i.InvoiceAmount
                 }).ToList()
            };
        }
    }
}
