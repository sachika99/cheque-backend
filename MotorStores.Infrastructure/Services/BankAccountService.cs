using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;
using MotorStores.Infrastructure.Persistence;
using System;

namespace MotorStores.Infrastructure.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IChequeBookService _chequeBookService;

        public BankAccountService(ApplicationDbContext context, IChequeBookService chequeBookService)
        {
            _context = context;
            _chequeBookService = chequeBookService;
        }

        public async Task<IEnumerable<BankAccountDto>> GetAllAsync()
        {
            var accounts = await _context.BankAccounts
                .Include(ba => ba.Bank)
                .OrderBy(ba => ba.AccountNo)
                .ToListAsync();

            return accounts.Select(MapToDto);
        }

        public async Task<IEnumerable<BankAccountDto>> GetByBankIdAsync(int bankId)
        {
            var accounts = await _context.BankAccounts
                .Include(ba => ba.Bank)
                .Where(ba => ba.BankId == bankId)
                .OrderBy(ba => ba.AccountNo)
                .ToListAsync();

            return accounts.Select(MapToDto);
        }

        public async Task<BankAccountDto?> GetByIdAsync(int id)
        {
            var account = await _context.BankAccounts
                .Include(ba => ba.Bank)
                .FirstOrDefaultAsync(ba => ba.Id == id);

            return account == null ? null : MapToDto(account);
        }

        public async Task<BankAccountDto> CreateAsync(BankAccountDto dto)
        {
            var bankExists = await _context.Banks.AnyAsync(b => b.Id == dto.BankId);
            if (!bankExists)
                throw new InvalidOperationException($"Bank with ID {dto.BankId} not found.");

            //var exists = await _context.BankAccounts
            //    .AnyAsync(ba => ba.AccountNo == dto.AccountNo);

            //if (exists)
            //    throw new InvalidOperationException($"Account with number {dto.AccountNo} already exists.");

            var account = new BankAccount
            {
                BankId = dto.BankId,
                AccountNo = dto.AccountNo,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                Balance = dto.Balance,
                Status = Enum.Parse<AccountStatus>(dto.Status),
                CreatedAt = DateTime.UtcNow
            };

            _context.BankAccounts.Add(account);
            await _context.SaveChangesAsync();

            await _context.Entry(account)
                .Reference(a => a.Bank)
                .LoadAsync();

            var chequeBookDto = new ChequeBookDto
            {
                BankAccountId = account.Id,
                ChequeBookNo = "001",
                StartChequeNo = 0,
                EndChequeNo = 0,
                CurrentChequeNo = 0,
                Status = "Active",
                IssuedDate = DateTime.UtcNow,
            };

            await _chequeBookService.CreateAsync(chequeBookDto);
            return MapToDto(account);
        }


        public async Task<BankAccountDto> UpdateAsync(BankAccountDto dto)
        {
            var account = await _context.BankAccounts
                .Include(ba => ba.Bank)
                .FirstOrDefaultAsync(ba => ba.Id == dto.Id);

            if (account == null)
                throw new InvalidOperationException($"Bank account with ID {dto.Id} not found.");

            // Validate bank exists if being changed
            if (account.BankId != dto.BankId)
            {
                var bankExists = await _context.Banks.AnyAsync(b => b.Id == dto.BankId);
                if (!bankExists)
                    throw new InvalidOperationException($"Bank with ID {dto.BankId} not found.");
            }

            // Check for duplicate account number (excluding current account)
            var exists = await _context.BankAccounts
                .AnyAsync(ba => ba.AccountNo == dto.AccountNo && ba.Id != dto.Id);

            if (exists)
                throw new InvalidOperationException($"Another account with number {dto.AccountNo} already exists.");

            account.BankId = dto.BankId;
            account.AccountNo = dto.AccountNo;
            account.AccountName = dto.AccountName;
            account.AccountType = dto.AccountType;
            account.Balance = dto.Balance;
            account.Status = Enum.Parse<AccountStatus>(dto.Status);
            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(account).Reference(a => a.Bank).LoadAsync();

            return MapToDto(account);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _context.BankAccounts
                .Include(ba => ba.ChequeBooks)
                .Include(ba => ba.Cheques)
                .FirstOrDefaultAsync(ba => ba.Id == id);

            if (account == null)
                return false;

            if (account.Cheques.Any())
                throw new InvalidOperationException(
                    "Cannot delete bank account because cheques are linked to this account."
                );

            if (account.ChequeBooks.Any())
            {
                _context.ChequeBooks.RemoveRange(account.ChequeBooks);
            }

            _context.BankAccounts.Remove(account);

            await _context.SaveChangesAsync();
            return true;
        }


        private BankAccountDto MapToDto(BankAccount account)
        {
            return new BankAccountDto
            {
                Id = account.Id,
                BankId = account.BankId,
                BankName = account.Bank?.BankName ?? "Unknown",
                BranchName = account.Bank?.BranchName ?? "Unknown",
                AccountNo = account.AccountNo,
                AccountName = account.AccountName,
                AccountType = account.AccountType,
                Balance = account.Balance,
                Status = account.Status.ToString()
            };
        }

        public async Task ActivateAccountAsync(int accountId)
        {
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                throw new InvalidOperationException($"Bank account with ID {accountId} not found.");

            // Deactivate all other accounts of the same bank
            var accountsInBank = await _context.BankAccounts
                .ToListAsync();

            foreach (var acc in accountsInBank)
            {
                acc.Status = acc.Id == accountId
                    ? AccountStatus.Active
                    : AccountStatus.Inactive;

                acc.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

    }
}
