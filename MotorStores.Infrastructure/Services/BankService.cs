using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;

namespace MotorStores.Infrastructure.Services
{
    public class BankService : IBankService
    {
        private readonly ApplicationDbContext _context;

        public BankService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            var banks = await _context.Banks
                .OrderBy(b => b.BankName)
                .ToListAsync();

            return banks.Select(MapToDto);
        }

        public async Task<BankDto?> GetByIdAsync(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            return bank == null ? null : MapToDto(bank);
        }

        public async Task<BankDto> CreateAsync(BankDto dto)
        {
            // Check for duplicate bank code
            var exists = await _context.Banks
                .AnyAsync(b => b.BranchCode == dto.BranchCode);

            if (exists)
                throw new InvalidOperationException($"Bank with branch code {dto.BranchCode} already exists.");

            var bank = new Bank
            {
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                BranchCode = dto.BranchCode,
                Status = Enum.Parse<BankStatus>(dto.Status),
                CreatedAt = DateTime.UtcNow
            };

            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();

            return MapToDto(bank);
        }

        public async Task<BankDto> UpdateAsync(BankDto dto)
        {
            var bank = await _context.Banks.FindAsync(dto.Id);

            if (bank == null)
                throw new InvalidOperationException($"Bank with ID {dto.Id} not found.");

            // Check for duplicate branch code (excluding current bank)
            var exists = await _context.Banks
                .AnyAsync(b => b.BranchCode == dto.BranchCode && b.Id != dto.Id);

            if (exists)
                throw new InvalidOperationException($"Another bank with branch code {dto.BranchCode} already exists.");

            bank.BankName = dto.BankName;
            bank.BranchName = dto.BranchName;
            bank.BranchCode = dto.BranchCode;
            bank.Status = Enum.Parse<BankStatus>(dto.Status);
            bank.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(bank);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bank = await _context.Banks
                .Include(b => b.BankAccounts)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bank == null)
                return false;

            // Prevent deletion if bank has associated accounts
            if (bank.BankAccounts.Any())
                throw new InvalidOperationException("Cannot delete bank with associated bank accounts.");

            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();

            return true;
        }

        private BankDto MapToDto(Bank bank)
        {
            return new BankDto
            {
                Id = bank.Id,
                BankName = bank.BankName,
                BranchName = bank.BranchName,
                BranchCode = bank.BranchCode,
                Status = bank.Status.ToString()
            };
        }
    }
}
