using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Services
{
    public class ChequeBookService : IChequeBookService
    {
        private readonly ApplicationDbContext _context;

        public ChequeBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChequeBookDto>> GetAllAsync()
        {
            var chequeBooks = await _context.ChequeBooks
                .Include(cb => cb.BankAccount)
                .OrderByDescending(cb => cb.IssuedDate)
                .ToListAsync();

            return chequeBooks.Select(MapToDto);
        }

        public async Task<IEnumerable<ChequeBookDto>> GetByBankAccountIdAsync(int bankAccountId)
        {
            var chequeBooks = await _context.ChequeBooks
                .Include(cb => cb.BankAccount)
                .Where(cb => cb.BankAccountId == bankAccountId)
                .OrderByDescending(cb => cb.IssuedDate)
                .ToListAsync();

            return chequeBooks.Select(MapToDto);
        }

        public async Task<ChequeBookDto?> GetByIdAsync(int id)
        {
            var chequeBook = await _context.ChequeBooks
                .Include(cb => cb.BankAccount)
                .FirstOrDefaultAsync(cb => cb.Id == id);

            return chequeBook == null ? null : MapToDto(chequeBook);
        }

        public async Task<ChequeBookDto> CreateAsync(ChequeBookDto dto)
        {
            var accountExists = await _context.BankAccounts.AnyAsync(ba => ba.Id == dto.BankAccountId);

            if (!accountExists)
                throw new InvalidOperationException($"Bank account with ID {dto.BankAccountId} not found.");
   

            var chequeBook = new ChequeBook
            {
                BankAccountId = dto.BankAccountId,
                ChequeBookNo = dto.ChequeBookNo,
                StartChequeNo = dto.StartChequeNo,
                EndChequeNo = dto.EndChequeNo,
                CurrentChequeNo = dto.StartChequeNo,
                Status = ChequeBookStatus.Active,
                IssuedDate = dto.IssuedDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChequeBooks.Add(chequeBook);
            await _context.SaveChangesAsync();

            await _context.Entry(chequeBook).Reference(cb => cb.BankAccount).LoadAsync();

            return MapToDto(chequeBook);
        }

        public async Task<ChequeBookDto> UpdateAsync(ChequeBookDto dto)
        {
            var chequeBook = await _context.ChequeBooks
                .Include(cb => cb.BankAccount)
                .FirstOrDefaultAsync(cb => cb.Id == dto.Id);

            if (chequeBook == null)
                throw new InvalidOperationException($"Cheque book with ID {dto.Id} not found.");

            // Validate bank account exists if being changed
            if (chequeBook.BankAccountId != dto.BankAccountId)
            {
                var accountExists = await _context.BankAccounts.AnyAsync(ba => ba.Id == dto.BankAccountId);
                if (!accountExists)
                    throw new InvalidOperationException($"Bank account with ID {dto.BankAccountId} not found.");
            }

            // Check for duplicate cheque book number (excluding current)
            var exists = await _context.ChequeBooks
                .AnyAsync(cb => cb.ChequeBookNo == dto.ChequeBookNo && cb.Id != dto.Id);

            if (exists)
                throw new InvalidOperationException($"Another cheque book with number {dto.ChequeBookNo} already exists.");

            // Validate cheque number range
            if (dto.StartChequeNo >= dto.EndChequeNo)
                throw new ArgumentException("Start cheque number must be less than end cheque number.");

            // Validate current cheque number is within range
            if (dto.CurrentChequeNo < dto.StartChequeNo || dto.CurrentChequeNo > dto.EndChequeNo)
                throw new ArgumentException("Current cheque number must be within the cheque book range.");

            chequeBook.BankAccountId = dto.BankAccountId;
            chequeBook.ChequeBookNo = dto.ChequeBookNo;
            chequeBook.StartChequeNo = dto.StartChequeNo;
            chequeBook.EndChequeNo = dto.EndChequeNo;
            chequeBook.CurrentChequeNo = dto.CurrentChequeNo;
            chequeBook.Status = Enum.Parse<ChequeBookStatus>(dto.Status);
            chequeBook.IssuedDate = dto.IssuedDate;
            chequeBook.UpdatedAt = DateTime.UtcNow;

            // Auto-complete if all cheques used
            if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
            {
                chequeBook.Status = ChequeBookStatus.Completed;
            }

            await _context.SaveChangesAsync();

            // Reload bank account if changed
            await _context.Entry(chequeBook).Reference(cb => cb.BankAccount).LoadAsync();

            return MapToDto(chequeBook);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var chequeBook = await _context.ChequeBooks
                .Include(cb => cb.Cheques)
                .FirstOrDefaultAsync(cb => cb.Id == id);

            if (chequeBook == null)
                return false;

            // Prevent deletion if cheque book has associated cheques
            if (chequeBook.Cheques.Any())
                throw new InvalidOperationException("Cannot delete cheque book with associated cheques.");

            _context.ChequeBooks.Remove(chequeBook);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetNextChequeNumberAsync(int chequeBookId)
        {
            var chequeBook = await _context.ChequeBooks.FindAsync(chequeBookId);

            if (chequeBook == null)
                throw new InvalidOperationException($"Cheque book with ID {chequeBookId} not found.");

            if (chequeBook.Status != ChequeBookStatus.Active)
                throw new InvalidOperationException($"Cheque book is not active.");

            if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
                throw new InvalidOperationException($"No more cheques available in this cheque book.");

            var nextChequeNo = chequeBook.CurrentChequeNo.ToString().PadLeft(6, '0');
            chequeBook.CurrentChequeNo++;

            // Auto-complete if last cheque
            if (chequeBook.CurrentChequeNo >= chequeBook.EndChequeNo)
            {
                chequeBook.Status = ChequeBookStatus.Completed;
            }

            chequeBook.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return nextChequeNo;
        }

        public async Task<ChequeBookDto> UpdateCurrentChequeNoAsync(int chequeBookId, int currentChequeNo)
        {
            var chequeBook = await _context.ChequeBooks
                .Include(cb => cb.BankAccount)
                .FirstOrDefaultAsync(cb => cb.Id == chequeBookId);

            if (chequeBook == null)
                throw new InvalidOperationException($"Cheque book with ID {chequeBookId} not found.");


            chequeBook.CurrentChequeNo = currentChequeNo;
            chequeBook.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(chequeBook);
        }

        private ChequeBookDto MapToDto(ChequeBook chequeBook)
        {
            return new ChequeBookDto
            {
                Id = chequeBook.Id,
                BankAccountId = chequeBook.BankAccountId,
                AccountNo = chequeBook.BankAccount?.AccountNo ?? "Unknown",
                ChequeBookNo = chequeBook.ChequeBookNo,
                StartChequeNo = chequeBook.StartChequeNo,
                EndChequeNo = chequeBook.EndChequeNo,
                CurrentChequeNo = chequeBook.CurrentChequeNo,
                Status = chequeBook.Status.ToString(),
                IssuedDate = chequeBook.IssuedDate
            };
        }
    }
}
