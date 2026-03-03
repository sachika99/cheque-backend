// Features/Banks/Commands/BankCommandHandlers.cs
using MediatR;
using MotorStores.Application.DTOs;
using MotorStores.Application.Interfaces;
using MotorStores.Application.Mappings;
using MotorStores.Domain.Entities;
using MotorStores.Domain.Enums;

namespace MotorStores.Application.Features.Banks.Commands;

// ── CREATE BANK ──────────────────────────────────────────────────────────────
public class CreateBankCommandHandler : IRequestHandler<CreateBankCommand, BankDto>
{
    private readonly IBankRepository _bankRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateBankCommandHandler(IBankRepository bankRepository, ICurrentUserService currentUser)
    {
        _bankRepository = bankRepository;
        _currentUser = currentUser;
    }

    public async Task<BankDto> Handle(CreateBankCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Bank;

            var branchCodeExists = await _bankRepository.BranchCodeExistsAsync(dto.BranchCode, cancellationToken);
            if (branchCodeExists)
                throw new InvalidOperationException($"Bank with branch code {dto.BranchCode} already exists.");

            var bankExists = await _bankRepository.BankAndBranchExistsAsync(dto.BankName, dto.BranchName, cancellationToken);
            if (bankExists)
                throw new InvalidOperationException($"Bank {dto.BankName} with branch {dto.BranchName} already exists.");

            var bank = new Bank
            {
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                BranchCode = dto.BranchCode,
                Status = Enum.Parse<BankStatus>(dto.Status),
                UserId = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _bankRepository.AddAsync(bank, cancellationToken);
            return BankMapper.ToDto(created);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}

// ── CREATE BANK WITH ACCOUNTS ─────────────────────────────────────────────────
public class CreateBankWithAccountsCommandHandler : IRequestHandler<CreateBankWithAccountsCommand, BankDto>
{
    private readonly IBankRepository _bankRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IChequeBookRepository _chequeBookRepository;

    public CreateBankWithAccountsCommandHandler(
        IBankRepository bankRepository,
        IBankAccountRepository bankAccountRepository,
        ICurrentUserService currentUser,
        IChequeBookRepository chequeBookRepository)
    {
        _bankRepository = bankRepository;
        _bankAccountRepository = bankAccountRepository;
        _currentUser = currentUser;
        _chequeBookRepository = chequeBookRepository;
    }

    public async Task<BankDto> Handle(CreateBankWithAccountsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.BankWithAccounts;

            var bank = new Bank
            {
                UserId = _currentUser.UserId,
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                BranchCode = dto.BranchCode,
                Status = Enum.Parse<BankStatus>(dto.Status),
                CreatedAt = DateTime.UtcNow
            };

            var savedBank = await _bankRepository.AddAsync(bank, cancellationToken);

            // ✅ Declared outside the loop so it's accessible after
            BankAccount? savedAccount = null;

            foreach (var acc in dto.BankAccounts)
            {
                var account = new BankAccount
                {
                    BankId = savedBank.Id,
                    AccountNo = acc.AccountNo,
                    AccountName = acc.AccountName,
                    AccountType = acc.AccountType,
                    Balance = acc.Balance,
                    Status = Enum.Parse<AccountStatus>(acc.Status),
                    UserId = _currentUser.UserId,
                    CreatedAt = DateTime.UtcNow,
                };

                savedAccount = await _bankAccountRepository.AddAsync(account, cancellationToken);
            }

            if (savedAccount is not null)
            {
                await _chequeBookRepository.AddAsync(new ChequeBook
                {
                    UserId = _currentUser.UserId,
                    BankAccountId = savedAccount.Id,
                    ChequeBookNo = "001",
                    StartChequeNo = 0,
                    EndChequeNo = 0,
                    CurrentChequeNo = "0",
                    Status = ChequeBookStatus.Active,
                    IssuedDate = DateTime.UtcNow
                });
            }

            return BankMapper.ToDto(savedBank);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}

// ── UPDATE BANK ──────────────────────────────────────────────────────────────
public class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, BankDto>
{
    private readonly IBankRepository _bankRepository;

    public UpdateBankCommandHandler(IBankRepository bankRepository)
        => _bankRepository = bankRepository;

    public async Task<BankDto> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Bank;

            var bank = await _bankRepository.GetByIdAsync(dto.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Bank with ID {dto.Id} not found.");

            var branchCodeExists = await _bankRepository.BranchCodeExistsAsync(dto.BranchCode, cancellationToken, excludeId: dto.Id);
            if (branchCodeExists)
                throw new InvalidOperationException($"Another bank with branch code {dto.BranchCode} already exists.");

            bank.BankName = dto.BankName;
            bank.BranchName = dto.BranchName;
            bank.BranchCode = dto.BranchCode;
            bank.Status = Enum.Parse<BankStatus>(dto.Status);
            bank.UpdatedAt = DateTime.UtcNow;

            var updated = await _bankRepository.UpdateAsync(bank, cancellationToken);
            return BankMapper.ToDto(updated);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}

// ── DELETE BANK ──────────────────────────────────────────────────────────────
public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand, bool>
{
    private readonly IBankRepository _bankRepository;

    public DeleteBankCommandHandler(IBankRepository bankRepository)
        => _bankRepository = bankRepository;

    public async Task<bool> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetByIdWithAccountsAsync(request.Id, cancellationToken);
            if (bank == null) return false;

            if (bank.BankAccounts.Any())
                throw new InvalidOperationException("Cannot delete bank with associated bank accounts.");

            await _bankRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
}